using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Filters;
using WebJob.Models;
using WebJob.Models.EF;
using WebJob.Models.Enum;
using WebJob.Models.ViewModels;

namespace WebJob.Controllers
{
    public class SaveJobController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: SaveJob
        public ActionResult Index()
        {
            SaveJob save = (SaveJob)Session["Save"];
            if (save != null)
            {
                return View(save.Items);
            }
            return View();
        }

        // Trang ứng tuyển
        [HttpGet]
        public ActionResult AppllyJob(int jobId)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra thông tin profile ứng viên
            var candidateProfile = db.CandidateProfiles
                .Include(c => c.Experience)
                .Include(c => c.Position)
                .Include(c => c.Location)
                .Include(c => c.JobCategory)
                .Include(c => c.CandidateProfileSkills)
                .FirstOrDefault(p => p.UserId.Equals(userId));

            if (candidateProfile == null)
            {
                return RedirectToAction("MyProfile", "Profile");
            }

            // Kiểm tra các thông tin quan trọng
            bool isProfileComplete = !string.IsNullOrEmpty(candidateProfile.CVPath) &&
                                    !string.IsNullOrEmpty(candidateProfile.AvatarPath) &&
                                    candidateProfile.ExperienceID != null &&
                                    candidateProfile.PositionID != null &&
                                    candidateProfile.LocationID != null &&
                                    candidateProfile.JobCategoryID != null &&
                                    candidateProfile.CandidateProfileSkills.Any();

            // Tạo model để gửi vào view
            var model = new ApplicationViewModel
            {
                JobID = jobId,
                IsProfileComplete = isProfileComplete
            };

            if (!isProfileComplete)
            {
                ViewBag.ProfileWarning = "Hãy cập nhật thông tin cá nhân đầy đủ để tăng cơ hội khi ứng tuyển";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult AppllyJob(ApplicationViewModel model, int jobId, HttpPostedFileBase CVFilePath)
        {
            var userId = User.Identity.GetUserId(); 
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để ứng tuyển." });
            }
            var candidateProfile = db.CandidateProfiles.FirstOrDefault(p => p.UserId.Equals(userId));
            if (candidateProfile == null)
            {
                return RedirectToAction("MyProfile", "Profile");
            }
            if (ModelState.IsValid && CVFilePath != null)
            {
                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };
                var fileExtension = Path.GetExtension(CVFilePath.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Chỉ chấp nhận file PDF, JPG, PNG, hoặc DOCX." });
                }

                // Kiểm tra kích thước file
                const int maxFileSize = 10 * 1024 * 1024; // 10 MB
                if (CVFilePath.ContentLength > maxFileSize)
                {
                    return Json(new { success = false, message = "Kích thước file không được vượt quá 10 MB." });
                }

                // Tạo tên file và đường dẫn để lưu
                string fileName = Path.GetFileName(CVFilePath.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Uploads/files"), fileName);

                try
                {
                    // Lưu file
                    CVFilePath.SaveAs(filePath);

                    // Kiểm tra xem ứng viên đã ứng tuyển công việc này chưa
                    bool alreadyApplied = db.JobApplications.Any(a => a.JobID == jobId && a.Applications.CandidateProfileID == candidateProfile.ID);
                    if (alreadyApplied)
                    {
                        return Json(new { success = false, message = "Bạn đã ứng tuyển công việc này trước đó." });
                    }

                    // Tạo đối tượng Applicant
                    var application = new Application
                    {
                        CandidateProfileID = candidateProfile.ID,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.Email,
                        CoverLetter = model.CoverLetter,
                        CVFilePath = "/Uploads/files/" + fileName,
                        CreatedBy = model.FullName,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        ViewStatus = 0,
                        ApplicationStatus = (int)ApplicationStatus.Pending,

                    };

                    db.Applications.Add(application);
                    db.SaveChanges();

                    // Tạo đối tượng JobApplication
                    var jobApplication = new JobApplication
                    {
                        JobID = jobId,
                        ApplicationID = application.ApplicationID,
                        CoverLetter = model.CoverLetter,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    db.JobApplications.Add(jobApplication);
                    db.SaveChanges();

                    // Lấy email nhà tuyển dụng
                    var job = db.Jobs.Include("Company").FirstOrDefault(j => j.JobID == jobId);
                    var recruiterEmail = job?.Company?.CompanyEmail;
                    string jobname = job?.JobTitle;

                    if (string.IsNullOrEmpty(recruiterEmail))
                    {
                        return Json(new { success = false, message = "Không tìm thấy email nhà tuyển dụng." });
                    }

                    // Đọc template email và gửi mail
                    string emailContent = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send3.html"));
                    emailContent = emailContent.Replace("{{FullName}}", model.FullName)
                                               .Replace("{{PhoneNumber}}", model.PhoneNumber)
                                               .Replace("{{CoverLetter}}", model.CoverLetter)
                                               .Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"))
                                               .Replace("{{CVLink}}", "/Uploads/files/" + fileName)
                                               .Replace("{{CVFileName}}", fileName)
                                               .Replace("{{JobName}}", jobname);

                    bool mailSent = WebJob.Models.Common.common.SendMail("Thông Tin Ứng Viên", "Ứng tuyển vị trí " + jobname, emailContent, recruiterEmail);

                    if (mailSent)
                    {
                        return Json(new { success = true, message = "Ứng tuyển thành công và email đã được gửi!", redirectUrl = Url.Action("Index", "viec-lam") });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Đã xảy ra lỗi khi gửi email. Vui lòng thử lại sau." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi trong quá trình ứng tuyển. Vui lòng thử lại sau." });
                }
            }

            return Json(new { success = false, message = "Thông tin ứng tuyển không hợp lệ hoặc không có file đính kèm." });
        }

        public ActionResult ShowCount()
        {
            SaveJob save = (SaveJob)Session["Save"];
            if (save != null)
            {
                return Json(new { Count = save.Items.Count }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddSaveJob(int id)
        {
            var code = new { success = false, msg = "", code = -1, Count = 0 };
            var db = new ApplicationDbContext();
            var checkJob = db.Jobs.FirstOrDefault(x => x.JobID == id);
            if (checkJob != null)
            {
                SaveJob save = (SaveJob)Session["Save"];
                if (save == null)
                {
                    save = new SaveJob();
                }
                SaveJobItem item = new SaveJobItem
                {
                    SaveJobId = checkJob.JobID,
                    SaveJobName = checkJob.JobTitle,
                    SaveJobCate = checkJob.JobCategory.CategoryName,
                    Alias = checkJob.Alias,
                    SaveJobSalaryMin = checkJob.Salary.SalaryMin,
                    SaveJobSalaryMax = checkJob.Salary.SalaryMax

                };
                if (checkJob.CompanyImages.FirstOrDefault(x => x.IsDefault) != null)
                {
                    item.SaveJobImg = checkJob.CompanyImages.FirstOrDefault(x => x.IsDefault).ImageURL;
                }

                save.AddSaveJob(item);
                Session["Save"] = save;
                code = new { success = true, msg = "Lưu thành công", code = 1, Count = save.Items.Count };

            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { success = false, msg = "", code = -1, Count = 0 };
            SaveJob save = (SaveJob)Session["Save"];
            if (save != null)
            {
                var checkJob = save.Items.FirstOrDefault(x => x.SaveJobId == id);
                if (checkJob != null)
                {
                    save.Remove(id);
                    code = new { success = true, msg = "", code = 1, Count = save.Items.Count };
                }
            }
            return Json(code);
        }


        public ActionResult GetSaveJobItems()
        {
            SaveJob save = (SaveJob)Session["Save"];
            if (save != null)
            {
                return PartialView("_SaveJobView", save.Items);
            }
            return PartialView("_SaveJobView", new List<SaveJobItem>()); // trả về danh sách rỗng nếu không có dữ liệu
        }

        [HttpPost]
        public ActionResult DeleteAllJobs()
        {
            SaveJob save = (SaveJob)Session["Save"];
            if (save != null)
            {
                save.ClearSave();
                return Json(new { success = true, message = "Đã xóa tất cả công việc đã lưu" });
            }
            return Json(new { success = false });
        }

    }
}