using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;
using PagedList;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using WebJob.Filters;
using WebJob.Models.ViewModels;
using WebJob.Models.Enum;

namespace WebJob.Areas.Employer.Controllers
{
    [CustomAuthorize(Area = "Employer", Roles = "Employer")]
    public class ApplicationController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Employer/Applicant
        public ActionResult Index(int? page, int? jobId)
        {
            var matchingService = new MatchingService(db);
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var jobs = db.Jobs.Where(j => j.UserId == userId).ToList();
            var jobIds = jobs.Select(j => j.JobID).ToList();

            var applications = db.Applications
                                 .Include("JobApplications.Job")
                                 .Where(a => a.JobApplications.Any(ja => jobIds.Contains(ja.JobID)) && a.ApplicationStatus == (int)ApplicationStatus.Pending)
                                 .OrderByDescending(x => x.CreatedDate)
                                 .ToList();

            if (jobId.HasValue)
            {
                applications = applications
                    .Where(x => x.JobApplications.Any(ja => ja.JobID == jobId.Value))
                    .ToList();
            }

            // Map sang ViewModel
            var applicationViewModels = applications.Select(app =>
            {
                var jobIdFromApp = app.JobApplications.FirstOrDefault()?.JobID ?? 0;
                var candidateProfileId = app.CandidateProfileID ?? 0;

                double score = 0;
                if (jobIdFromApp > 0 && candidateProfileId > 0)
                {
                    var matching = matchingService.CalculateMatchingScore(jobIdFromApp, candidateProfileId);
                    score = matching?.Score ?? 0;
                }

                return new ApplicationViewModel
                {
                    ApplicationID = app.ApplicationID,
                    JobID = jobIdFromApp,
                    JobName = app.JobApplications.FirstOrDefault()?.Job?.JobTitle,
                    FullName = app.FullName,
                    Email = app.Email,
                    PhoneNumber = app.PhoneNumber,
                    CoverLetter = app.CoverLetter,
                    CVFilePath = app.CVFilePath,
                    IsProfileComplete = app.CandidateProfile != null,
                    MatchingScore = score,
                };
            }).ToList();

            var uniqueJobs = applications
                .SelectMany(x => x.JobApplications)
                .Select(ja => ja.Job)
                .GroupBy(j => j.JobID)
                .Select(g => g.First())
                .Where(j => j.EndDate >= DateTime.Now)
                .ToList();

            ViewBag.UniqueJobs = uniqueJobs;
            ViewBag.SelectedJobId = jobId;

            var pageNumber = page ?? 1;
            var pageSize = 8;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;

            return View(applicationViewModels.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult ApplicantsByJob(int jobId, int? page)
        {
            // Lấy ID người dùng hiện tại
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                // Nếu chưa đăng nhập, chuyển hướng người dùng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Lấy công việc theo JobID và UserId (nếu cần)
            var job = db.Jobs
                        .Include("JobApplications.Applications")
                        .FirstOrDefault(j => j.JobID == jobId && j.UserId == userId); 

            if (job == null || job.EndDate < DateTime.Now) // Kiểm tra nếu công việc đã hết hạn hoặc không tồn tại
            {
                return HttpNotFound();
            }

            // Lấy danh sách ứng viên của công việc
            var applicants = job.JobApplications.Select(ja => ja.Applications)
                                                .OrderByDescending(a => a.CreatedDate)
                                                .ToList();

            // Phân trang
            var pageNumber = page ?? 1;
            var pageSize = 8;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;

            
            ViewBag.UniqueJobs = db.Jobs.Where(j => j.UserId == userId && j.EndDate >= DateTime.Now).ToList();

            // Truyền thông tin công việc hiện tại vào ViewBag
            ViewBag.SelectedJobId = jobId;
            ViewBag.JobTitle = job.JobTitle; // Tên công việc

            return View(applicants.ToPagedList(pageNumber, pageSize));
        }


        // Action GET: Hiển thị thông tin ứng viên và nhận xét
        public ActionResult View(int id)
        {
            var applicant = db.Applications.Find(id);

            if (applicant != null)
            {
                // Lọc công việc có ngày hết hạn chưa qua
                var jobs = db.Jobs.Include("JobApplications")
                                  .Where(j => j.EndDate >= DateTime.Now)
                                  .ToList();
                ViewBag.UniqueJobs = jobs;
                applicant.ViewStatus = 1; // Đánh dấu là đã xem
                db.SaveChanges();
            }

            return View(applicant);
        }

        // Cập nhật nhận xét 
        [HttpPost]
        public ActionResult View(int id, string feedback)
        {
            var application = db.Applications.Find(id);

            if (application != null)
            {

                application.FeebBack = feedback;
                db.SaveChanges();
            }


            return RedirectToAction("View", new { id = id });
        }
        [HttpPost]
        public ActionResult AutoEvaluate(int? jobId)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách ứng viên có điểm < 50
            var applications = db.Applications
                                .Include("JobApplications.Job")
                                .Where(a => a.JobApplications.Any(ja => ja.Job.UserId == userId) &&
                                           a.ApplicationStatus == (int)ApplicationStatus.Pending)
                                .ToList();

            if (jobId.HasValue)
            {
                applications = applications
                    .Where(a => a.JobApplications.Any(ja => ja.JobID == jobId.Value))
                    .ToList();
            }

            var matchingService = new MatchingService(db);
            int rejectedCount = 0;

            foreach (var app in applications)
            {
                var jobIdFromApp = app.JobApplications.FirstOrDefault()?.JobID ?? 0;
                var candidateProfileId = app.CandidateProfileID ?? 0;

                if (jobIdFromApp > 0 && candidateProfileId > 0)
                {
                    var matching = matchingService.CalculateMatchingScore(jobIdFromApp, candidateProfileId);
                    double score = matching?.Score ?? 0;

                    if (score < 65)
                    {
                        app.ApplicationStatus = (int)ApplicationStatus.Rejected;
                        app.FeebBack = "Xin lỗi hồ sơ của bạn không phù hợp với chúng tôi";
                        rejectedCount++;
                    }
                }
            }

            if (rejectedCount > 0)
            {
                db.SaveChanges();
                TempData["AutoEvaluateMessage"] = $"Đã tự động từ chối {rejectedCount} ứng viên không phù hợp.";
            }
            else
            {
                TempData["AutoEvaluateMessage"] = "Không có ứng viên nào bị từ chối tự động.";
            }

            return RedirectToAction("Index", new { jobId = jobId });
        }
    }
}