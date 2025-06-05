using PagedList;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebJob.Filters;
using WebJob.Models;
using WebJob.Models.EF;
using WebJob.Models.Enum;

namespace WebJob.Areas.Admin.Controllers
{
    [CustomAuthorize(Area = "Admin", Roles = "Admin")]
    public class VerifyAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/VerifyAccount
        public ActionResult Index(int? page, string filter = "all")
        {
            // Lấy danh sách từ cơ sở dữ liệu
            var query = db.employerVerifications.AsQueryable();

            if (filter == "approved")
            {
                query = query.Where(x => x.Status == (int)VerificationStatus.Approved);
            }
            else if (filter == "pending")
            {
                query = query.Where(x => x.Status == (int)VerificationStatus.Pending);
            }
            else if (filter == "rejected")
            {
                query = query.Where(x => x.Status == (int)VerificationStatus.Rejected);
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;
            var model = query.OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize);
            ViewBag.Filter = filter;

            return View(model);
        }

        // GET: Admin/VerifyAccount/Details/5
        public ActionResult Details(int id)
        {
            var account = db.employerVerifications.FirstOrDefault(e => e.Id == id);

            if (account == null)
            {
                return HttpNotFound("Không tìm thấy tài khoản.");
            }

            // Kiểm tra và đảm bảo đường dẫn tài liệu có thể truy cập từ trình duyệt (URL tương đối)
            if (!string.IsNullOrEmpty(account.VerificationDocumentPath))
            {
                account.VerificationDocumentPath = Url.Content("~/Uploads/VerificationDocuments/" + Path.GetFileName(account.VerificationDocumentPath));
            }

            return View(account);
        }



        //// POST: Admin/VerifyAccount/Verify/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Verify(int id)
        //{
        //    // Lấy tài khoản từ cơ sở dữ liệu
        //    var account = db.employerVerifications.FirstOrDefault(e => e.Id == id);
        //    if (account == null)
        //    {
        //        return HttpNotFound("Không tìm thấy tài khoản.");
        //    }

        //    // Cập nhật trạng thái xác thực
        //    account.IsVerified = true;

        //    // Lưu thay đổi vào cơ sở dữ liệu
        //    db.SaveChanges();

        //    // Quay lại trang chi tiết hoặc danh sách
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            var account = db.employerVerifications.FirstOrDefault(e => e.Id == id);
            if (account == null)
            {
                return HttpNotFound("Không tìm thấy tài khoản.");
            }

            account.Status = (int)VerificationStatus.Approved;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id)
        {
            var account = db.employerVerifications.FirstOrDefault(e => e.Id == id);
            if (account == null)
            {
                return HttpNotFound("Không tìm thấy tài khoản.");
            }

            account.Status = (int)VerificationStatus.Rejected;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ChangeStatus(int id)
        //{
        //    var account = db.employerVerifications.FirstOrDefault(e => e.Id == id);
        //    if (account == null)
        //    {
        //        return HttpNotFound("Không tìm thấy tài khoản.");
        //    }

        //    // Thay đổi trạng thái xác thực (đảo ngược trạng thái)
        //    account.IsVerified = !account.IsVerified;

        //    // Lưu thay đổi vào cơ sở dữ liệu
        //    db.SaveChanges();

        //    // Quay lại trang chi tiết tài khoản
        //    return RedirectToAction("Details", new { id = id });
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(int id, int newStatus)
        {
            var account = db.employerVerifications.FirstOrDefault(e => e.Id == id);
            if (account == null)
            {
                return HttpNotFound("Không tìm thấy tài khoản.");
            }

            // Validate status value
            if (newStatus < 0 || newStatus > 2)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Trạng thái không hợp lệ");
            }

            account.Status = newStatus;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }
    }
}
