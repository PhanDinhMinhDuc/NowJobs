using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using WebJob.Models.ViewModels;


namespace WebJob.Controllers
{
    public class MyApplicantController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyApplicant
        public ActionResult Index()
        {
            return View();
        }
        // GET: MyApplicant
        public ActionResult MyAppliedJobs(int? page)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var candidateProfile = db.CandidateProfiles.FirstOrDefault(p => p.UserId.Equals(userId));

            // Lấy danh sách công việc đã ứng tuyển với đầy đủ thông tin
            var appliedJobs = db.JobApplications
                                .Where(ja => ja.Applications.CandidateProfileID == candidateProfile.ID)
                                .Select(ja => new MyAppliedJobViewModel
                                {
                                    JobId = ja.Job.JobID,
                                    JobTitle = ja.Job.JobTitle,
                                    CompanyName = ja.Job.Company.CompanyName,
                                    CompanyLogo = ja.Job.CompanyImages
                                                      .Where(img => img.IsDefault)
                                                      .Select(img => img.ImageURL)
                                                      .FirstOrDefault(),
                                    SalaryRange = ja.Job.Salary.SalaryRange,
                                    PositionName = ja.Job.Position.PositionName,
                                    CreatedDate = ja.Applications.CreatedDate,
                                    EndDate = ja.Job.EndDate,
                                    FeebBack = ja.Applications.FeebBack,
                                    ApplicationStatus = ja.Applications.ApplicationStatus,
                                    Alias = ja.Job.Alias,
                                    LocationName = ja.Job.Location.LocationName
                                })
                                .OrderByDescending(j => j.CreatedDate)
                                .ToList();

            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;

            return View(appliedJobs.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int jobId)
        {
            var userId = User.Identity.GetUserId();
            var candidateProfile = db.CandidateProfiles.FirstOrDefault(p => p.UserId.Equals(userId));

            var jobApplication = db.JobApplications
                                   .Where(ja => ja.JobID == jobId && ja.Applications.CandidateProfileID == candidateProfile.ID)
                                   .FirstOrDefault();

            var feedback = jobApplication?.Applications.FeebBack;
            var status = jobApplication?.Applications.ApplicationStatus ?? 0;

            ViewBag.ApplicationStatus = status;
            return PartialView("_Feedback", feedback);
        }
        [HttpPost]
        public ActionResult CancelApplication(int jobId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này" });
                }

                var candidateProfile = db.CandidateProfiles.FirstOrDefault(p => p.UserId.Equals(userId));
                if (candidateProfile == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ ứng viên" });
                }

                // Tìm application để hủy
                var application = db.JobApplications
                                   .FirstOrDefault(ja => ja.JobID == jobId &&
                                                       ja.Applications.CandidateProfileID == candidateProfile.ID);

                if (application == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn ứng tuyển" });
                }

                // Xóa application
                db.JobApplications.Remove(application);
                db.SaveChanges();

                return Json(new { success = true, message = "Hủy ứng tuyển thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi hủy ứng tuyển: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CancelAllApplications()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này" });
                }

                var candidateProfile = db.CandidateProfiles.FirstOrDefault(p => p.UserId.Equals(userId));
                if (candidateProfile == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ ứng viên" });
                }

                // Lấy tất cả applications của user
                var applications = db.JobApplications
                                   .Where(ja => ja.Applications.CandidateProfileID == candidateProfile.ID)
                                   .ToList();

                if (!applications.Any())
                {
                    return Json(new { success = false, message = "Không có đơn ứng tuyển nào để hủy" });
                }

                // Xóa tất cả applications
                db.JobApplications.RemoveRange(applications);
                db.SaveChanges();

                return Json(new { success = true, message = "Đã hủy tất cả đơn ứng tuyển thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi hủy tất cả ứng tuyển: " + ex.Message });
            }
        }



    }
}