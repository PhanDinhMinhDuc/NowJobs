using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;
using WebJob.Models.EF;
using Microsoft.AspNet.Identity;
using System.Linq;
using WebJob.Models.Enum;
using WebJob.Filters;

namespace WebJob.Areas.Employer.Controllers
{
    [CustomAuthorize(Area = "Employer", Roles = "Employer")]
    public class EmployerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employer/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Employer/VerificationDetails
        [HttpGet]
        public ActionResult VerificationDetails()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var verification = db.employerVerifications
                                     .FirstOrDefault(v => v.UserId == userId);
                var currentUser = db.Users.Find(userId);

                ViewBag.Companies = db.Companies.OrderBy(c => c.CompanyName).ToList();

                if (verification != null)
                {
                    EmployerVerificationViewModel model = new EmployerVerificationViewModel
                    {
                        CompanyId = verification.CompanyId,
                        CompanyName = verification.CompanyName,
                        PhoneNumber = verification.PhoneNumber,
                        Email = verification.Email,
                        BusinessLicenseNumber = verification.BusinessLicenseNumber,
                        VerificationDocumentPath = verification.VerificationDocumentPath,
                        UserId = verification.UserId,
                        CreatedDate = verification.CreatedDate,
                        Company = verification.Company,
                        EmployerName = currentUser?.FullName // tránh lỗi nếu currentUser null
                    };

                    switch ((VerificationStatus)verification.Status)
                    {
                        case VerificationStatus.Approved:
                            model.Status = (int)VerificationStatus.Approved;
                            break;
                        case VerificationStatus.Pending:
                            model.Status = (int)VerificationStatus.Pending;
                            break;
                        case VerificationStatus.Rejected:
                            model.Status = (int)VerificationStatus.Rejected;
                            break;
                    }

                    ViewBag.Status = model.Status;
                    return View(model);
                }

                // Nếu không có bản ghi nào, trả về view rỗng kèm ViewBag
                ViewBag.Status = (int)VerificationStatus.Pending;
                return View();
            }
            catch (Exception ex)
            {
                // Ghi log nếu có hệ thống logging (nlog, serilog, v.v.)
                // Logger.Error(ex);

                // Thông báo lỗi nhẹ nhàng
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình lấy thông tin xác minh. Vui lòng thử lại sau.";
                return View("Error"); // hoặc trả về view mặc định với thông báo lỗi
            }
        }
        // POST: Employer/VerifyEmployer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyEmployer(EmployerVerificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem email đã tồn tại trong cơ sở dữ liệu chưa
                    bool emailExists = db.employerVerifications.Any(e => e.Email == model.Email);
                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                        ViewBag.Companies = db.Companies.OrderBy(c => c.CompanyName).ToList();
                        return View("VerificationDetails", model);
                    }

                    if (model.VerificationDocument != null && model.VerificationDocument.ContentLength > 0)
                    {
                        // Kiểm tra định dạng và kích thước file
                        string fileExtension = Path.GetExtension(model.VerificationDocument.FileName)?.ToLower();
                        string[] allowedExtensions = { ".pdf", ".jpg", ".png" };

                        if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                        {
                            ModelState.AddModelError("VerificationDocument", "Chỉ chấp nhận các định dạng file: PDF, JPG, PNG.");
                            ViewBag.Companies = db.Companies.OrderBy(c => c.CompanyName).ToList();
                            return View("VerificationDetails", model);
                        }

                        if (model.VerificationDocument.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("VerificationDocument", "Kích thước file không được vượt quá 5 MB.");
                            ViewBag.Companies = db.Companies.OrderBy(c => c.CompanyName).ToList();
                            return View("VerificationDetails", model);
                        }

                        // Lưu file vào thư mục
                        string uploadDir = Server.MapPath("~/Uploads/VerificationDocuments");
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                        // Tạo tên file duy nhất
                        string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        string filePath = Path.Combine(uploadDir, uniqueFileName);

                        // Lưu file vào thư mục
                        model.VerificationDocument.SaveAs(filePath);

                        // Lưu đường dẫn tương đối của file vào cơ sở dữ liệu
                        string relativeFilePath = "~/Uploads/VerificationDocuments/" + uniqueFileName;

                        // Xử lý Company
                        Company company;
                        if (model.CompanyId.HasValue && model.CompanyId.Value > 0)
                        {
                            // Nếu chọn từ dropdown
                            company = db.Companies.Find(model.CompanyId.Value);
                            if (company == null)
                            {
                                ModelState.AddModelError("CompanyName", "Công ty không tồn tại");
                                ViewBag.Companies = db.Companies.OrderBy(c => c.CompanyName).ToList();
                                return View("VerificationDetails", model);
                            }
                        }
                        else
                        {
                            // Tạo công ty mới
                            company = new Company
                            {
                                CompanyName = model.CompanyName,
                                CompanyEmail = model.Email,
                                CompanyAddress = "", // Có thể thêm trường address nếu cần
                                IsActive = true,
                                CreatedDate = DateTime.Now
                            };
                            db.Companies.Add(company);
                            db.SaveChanges(); // Lưu để lấy ID
                        }

                        // Lưu thông tin vào cơ sở dữ liệu
                        var verification = new EmployerVerification
                        {
                            CompanyId = company.CompanyID,
                            CompanyName = company.CompanyName,
                            PhoneNumber = model.PhoneNumber,
                            Email = model.Email,
                            BusinessLicenseNumber = model.BusinessLicenseNumber,
                            VerificationDocumentPath = relativeFilePath,
                            UserId = User.Identity.GetUserId(),
                            Status = (int)VerificationStatus.Pending,
                            CreatedDate = DateTime.Now
                        };

                        db.employerVerifications.Add(verification);
                        db.SaveChanges();

                        // Xác nhận gửi thông tin thành công
                        TempData["SuccessMessage"] = "Thông tin xác thực đã được gửi. Chúng tôi sẽ xử lý trong thời gian sớm nhất.";
                        return RedirectToAction("Confirmation");
                    }
                    else
                    {
                        ModelState.AddModelError("VerificationDocument", "Vui lòng tải lên tài liệu xác thực.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại.");
                }
            }

            ViewBag.Companies = db.Companies.OrderBy(c => c.CompanyName).ToList();
            return View("VerificationDetails", model);
        }
        [HttpGet]
        public JsonResult SearchCompanies(string term)
        {
            var companies = db.Companies
                             .Where(c => c.CompanyName.Contains(term))
                             .OrderBy(c => c.CompanyName)
                             .Select(c => new
                             {
                                 c.CompanyID,
                                 c.CompanyName
                             })
                             .ToList();

            return Json(companies, JsonRequestBehavior.AllowGet);
        }
        // GET: Employer/Confirmation
        public ActionResult Confirmation()
        {
            return View();
        }
    }
}
