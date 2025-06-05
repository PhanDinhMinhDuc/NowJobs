using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebJob.Filters;
using WebJob.Models;
using WebJob.Models.EF;
using WebJob.Models.Enum;
using WebJob.Models.ViewModels;

namespace WebJob.Areas.Employer.Controllers
{
    [CustomAuthorize(Area = "Employer", Roles = "Employer")]
    public class JobsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? page)
        {
            // Lấy ID người dùng đang đăng nhập
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                // Chuyển hướng người dùng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Lọc danh sách công việc theo UserId
            IEnumerable<Job> items = db.Jobs
                .Where(x => x.UserId == userId) // Lọc theo UserId của tài khoản đăng nhập
                .OrderByDescending(x => x.JobID);

            // Thiết lập phân trang
            //var pageSize = 10;
            //var pageIndex = page ?? 1; // Nếu page là null, gán mặc định là 1

            // Kiểm tra xem có công việc nào hay không
            if (!items.Any())
            {
                ViewBag.NoJobs = "Không có công việc nào được lưu bởi tài khoản này.";
            }

            var pageNumber = page ?? 1;
            var pageSize = 8;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));


            /*// Phân trang
            items = items.ToPagedList(pageNumber, pageSize);


            // Truyền dữ liệu vào View
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;

            return View(items);*/
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy thông tin job với các quan hệ liên quan
            var job = db.Jobs
                .Include(j => j.Company)
                .Include(j => j.Salary)
                .Include(j => j.Experience)
                .Include(j => j.Location)
                .Include(j => j.JobCategory)
                .Include(j => j.JobJobSkills)
                .Include(j => j.JobJobSkills.Select(js => js.JobSkill))
                .Include(j => j.CompanyImages)
                .FirstOrDefault(j => j.JobID == id);

            if (job == null)
            {
                return HttpNotFound();
            }

            // Tính số ngày còn lại đến hạn
            if (job.EndDate != null)
            {
                ViewBag.DaysRemaining = (job.EndDate - DateTime.Now).Days;
            }

            return View(job);
        }
        public ActionResult Add()
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                // Chuyển hướng người dùng đến trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("Login", "Account");
            }
            // Kiểm tra và thêm dữ liệu mặc định vào bảng Experience nếu chưa có
            if (!db.Experiences.Any())
            {
                var experienceLevels = new List<Experience>
        {
            new Experience { ExperienceLevel = "Chưa có kinh nghiệm" },
            new Experience { ExperienceLevel = "Dưới 1 năm" },
            new Experience { ExperienceLevel = "2 năm" },
            new Experience { ExperienceLevel = "3 năm" },
            new Experience { ExperienceLevel = "4-5 năm" },
            new Experience { ExperienceLevel = "Trên 5 năm" }
        };

                db.Experiences.AddRange(experienceLevels);
                db.SaveChanges();
            }
            //
            if (!db.Salaries.Any())
            {
                var salaryRanges = new List<Salary>
                {
                    new Salary { SalaryRange = "Dưới 10 triệu", SalaryMin = 0, SalaryMax = 9999999 },
                    new Salary { SalaryRange = "Từ 10-15 triệu", SalaryMin = 10000000, SalaryMax = 15000000 },
                    new Salary { SalaryRange = "Từ 15-20 triệu", SalaryMin = 15000000, SalaryMax = 20000000 },
                    new Salary { SalaryRange = "Từ 20-25 triệu", SalaryMin = 20000000, SalaryMax = 25000000 },
                    new Salary { SalaryRange = "Từ 25-30 triệu", SalaryMin = 25000000, SalaryMax = 30000000 },
                    new Salary { SalaryRange = "Từ 30-50 triệu", SalaryMin = 30000000, SalaryMax = 50000000 },
                    new Salary { SalaryRange = "Trên 50 triệu", SalaryMin = 50000000, SalaryMax = int.MaxValue }
                };
                db.Salaries.AddRange(salaryRanges);
                db.SaveChanges();
            }

            var model = new JobViewModel
            {
                JobSkillsList = new MultiSelectList(db.JobSkills.ToList(), "Id", "JobSkillName"),
                JobCategories = new SelectList(db.JobCategories.ToList(), "JobCategoryID", "CategoryName"),
                Experiences = new SelectList(db.Experiences.ToList(), "ExperienceID", "ExperienceLevel"),
                Salaries = new SelectList(db.Salaries.ToList(), "SalaryID", "SalaryRange"),
                Locations = new SelectList(db.Locations.Where(p=>p.IsActive).ToList(),"LocationID","LocationName"),
                Positions = new SelectList(db.Positions.Where(p=>p.IsActive).ToList(),"PositionID","PositionName")
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(JobViewModel model, List<string> Images, List<int> rDefault)
        {
            // Kiểm tra người dùng đã đăng nhập hay chưa
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Bạn cần đăng nhập trước khi thêm công việc.");
                return RedirectToAction("Login", "Account", new { area = "Employer" }); // Chuyển hướng người dùng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy thông tin tài khoản đăng nhập từ bảng EmployerVerifications
            var employer = db.employerVerifications.FirstOrDefault(e => e.UserId == userId);
            if (employer == null || employer.Status == (int)VerificationStatus.Pending || employer.Status == (int)VerificationStatus.Rejected)
            {
                ModelState.AddModelError("", "Tài khoản của bạn chưa được xác thực. Vui lòng xác thực tài khoản trước khi đăng tin.");
                return RedirectToAction("VerifyEmployerForm", "Employer");
            }
            if (!employer.IsActive())
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn đã hết hạn hoặc đã sử dụng hết lượt đăng tin. Vui lòng mua thêm gói đăng tin để tiếp tục.";
                return RedirectToAction("Package", "Employer");
            }
            var companyUpdate = model.Company;
            model.Company = null;
            // Kiểm tra và thêm dữ liệu mặc định vào bảng Experience
            if (!db.Experiences.Any())
            {
                var experienceLevels = new List<Experience>
        {
            new Experience { ExperienceLevel = "Chưa có kinh nghiệm" },
            new Experience { ExperienceLevel = "Dưới 1 năm" },
            new Experience { ExperienceLevel = "2 năm" },
            new Experience { ExperienceLevel = "3 năm" },
            new Experience { ExperienceLevel = "4-5 năm" },
            new Experience { ExperienceLevel = "Trên 5 năm" }
        };

                db.Experiences.AddRange(experienceLevels);
                db.SaveChanges();
            }
            // Tạo dữ liệu mặc định cho bảng Salary nếu chưa có
            if (!db.Salaries.Any())
            {
                var salaryRanges = new List<Salary>
                {
                    new Salary { SalaryRange = "Dưới 10 triệu", SalaryMin = 0, SalaryMax = 9999999 },
                    new Salary { SalaryRange = "Từ 10-15 triệu", SalaryMin = 10000000, SalaryMax = 15000000 },
                    new Salary { SalaryRange = "Từ 15-20 triệu", SalaryMin = 15000000, SalaryMax = 20000000 },
                    new Salary { SalaryRange = "Từ 20-25 triệu", SalaryMin = 20000000, SalaryMax = 25000000 },
                    new Salary { SalaryRange = "Từ 25-30 triệu", SalaryMin = 25000000, SalaryMax = 30000000 },
                    new Salary { SalaryRange = "Từ 30-50 triệu", SalaryMin = 30000000, SalaryMax = 50000000 },
                    new Salary { SalaryRange = "Trên 50 triệu", SalaryMin = 50000000, SalaryMax = int.MaxValue }
                };
                db.Salaries.AddRange(salaryRanges);
                db.SaveChanges();
            }

            // Nếu ModelState hợp lệ, tiếp tục xử lý công việc
            if (ModelState.IsValid)
            {
                var job = new Job
                {
                    JobTitle = model.JobTitle,
                    JobDescription = model.JobDescription,
                    JobRequirements = model.JobRequirements,
                    JobBenefits = model.JobBenefits,
                    JobCategoryID = model.JobCategoryID,
                    ExperienceID = model.ExperienceID,
                    SalaryID = model.SalaryID,
                    LocationID = model.LocationID,
                    PositionID = model.PositionID,
                    IsActive = false,
                    IsNow = model.IsNow,
                    EndDate = model.EndDate,
                    UserId = userId,
                    CompanyID = employer.CompanyId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ViewCount = 0,
                    CreatedBy = User.Identity.GetUserId(),
                    Alias = string.IsNullOrEmpty(model.Alias) ? WebJob.Models.Common.Filter.FilterChar(model.JobTitle) : model.Alias
                };
                if (model.SelectedJobSkills != null && model.SelectedJobSkills.Any())
                {
                    foreach (var skillId in model.SelectedJobSkills)
                    {
                        job.JobJobSkills.Add(new Job_JobSkill
                        {
                            JobSkillId = skillId
                        });
                    }
                }
                model.CompanyID = employer.CompanyId;
                // Xử lý hình ảnh nếu có
                if (Images != null && Images.Count > 0)
                {
                    foreach (var imageUrl in Images)
                    {
                        job.CompanyImages.Add(new CompanyImage
                        {
                            ImageURL = imageUrl,
                            IsDefault = rDefault != null && rDefault.Contains(Images.IndexOf(imageUrl) + 1),
                            CreatedBy = User.Identity.GetUserName(),
                            CreatedDate = DateTime.Now,
                            CompanyID = Convert.ToInt32(employer.CompanyId)
                        });
                    }
                }
                // Cập nhật thông tin công ty nếu có thay đổi
                if (companyUpdate != null)
                {
                    var existingCompany = db.Companies.Find(employer.CompanyId);
                    if (existingCompany != null)
                    {
                        // Chỉ cập nhật các trường thay đổi
                        if (!string.IsNullOrEmpty(companyUpdate.CompanyName))
                            existingCompany.CompanyName = companyUpdate.CompanyName;

                        if (!string.IsNullOrEmpty(companyUpdate.CompanyEmail))
                            existingCompany.CompanyEmail = companyUpdate.CompanyEmail;

                        if (!string.IsNullOrEmpty(companyUpdate.CompanyDescription))
                            existingCompany.CompanyDescription = companyUpdate.CompanyDescription;

                        if (!string.IsNullOrEmpty(companyUpdate.CompanyAddress))
                            existingCompany.CompanyAddress = companyUpdate.CompanyAddress;

                        existingCompany.ModifiedDate = DateTime.Now;
                    }
                }
                // Lưu công việc vào cơ sở dữ liệu
                db.Jobs.Add(job);
                db.SaveChanges();
                employer.PostRemain--;
                db.Entry(employer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu không hợp lệ, trả về View và giữ lại dữ liệu đã nhập
            model.JobSkillsList = new MultiSelectList(db.JobSkills.ToList(), "Id", "JobSkillName", model.SelectedJobSkills);
            model.JobCategories = new SelectList(db.JobCategories.ToList(), "JobCategoryID", "CategoryName");
            model.Experiences = new SelectList(db.Experiences.ToList(), "ExperienceID", "ExperienceLevel");
            model.Salaries = new SelectList(db.Salaries.ToList(), "SalaryID", "SalaryRange");
            model.Locations = new SelectList(db.Locations.ToList(), "LocationID", "LocationName");
            model.Positions = new SelectList(db.Locations.Where(p => p.IsActive).ToList(), "PositionID", "PositionName");

            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Load job với các quan hệ cần thiết
            var jobInDb = db.Jobs
                           .Include(j => j.JobJobSkills)
                           .Include(j => j.Location)
                           .Include(j => j.CompanyImages)
                           .FirstOrDefault(j => j.JobID == id);

            var company = db.Companies.FirstOrDefault(j => j.CompanyID == jobInDb.CompanyID);
            if (jobInDb == null)
            {
                return HttpNotFound();
            }

            // Tạo ViewModel từ Job
            var model = new JobViewModel
            {
                JobID = jobInDb.JobID,
                JobTitle = jobInDb.JobTitle,
                JobDescription = jobInDb.JobDescription,
                JobRequirements = jobInDb.JobRequirements,
                JobBenefits = jobInDb.JobBenefits,
                JobCategoryID = jobInDb.JobCategoryID,
                ExperienceID = jobInDb.ExperienceID,
                SalaryID = jobInDb.SalaryID,
                LocationID = jobInDb.LocationID,
                IsActive = jobInDb.IsActive,
                IsNow = jobInDb.IsNow,
                EndDate = jobInDb.EndDate,
                Alias = jobInDb.Alias,
                SelectedJobSkills = jobInDb.JobJobSkills.Select(j => j.JobSkillId).ToArray(),
                CompanyName = company.CompanyName,
                CompanyEmail = company.CompanyEmail,
                CompanyDescription = company.CompanyDescription,
                CompanyAddress = company.CompanyAddress,
                CompanyImages = jobInDb.CompanyImages.ToList() // Thêm hình ảnh vào ViewModel
            };

            // Load các dropdown list
            model.Salaries = new SelectList(db.Salaries.ToList(), "SalaryID", "SalaryRange", jobInDb.SalaryID);
            model.Experiences = new SelectList(db.Experiences.ToList(), "ExperienceID", "ExperienceLevel", jobInDb.ExperienceID);
            model.JobCategories = new SelectList(db.JobCategories.ToList(), "JobCategoryID", "CategoryName", jobInDb.JobCategoryID);
            model.JobSkillsList = new MultiSelectList(db.JobSkills.ToList(), "Id", "JobSkillName", model.SelectedJobSkills);
            model.Locations = new SelectList(db.Locations.ToList(), "LocationID", "LocationName");
            model.Positions = new SelectList(db.Locations.Where(p => p.IsActive).ToList(), "PositionID", "PositionName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(JobViewModel model)
        {
            var userId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var jobInDb = db.Jobs
                               .Include(j => j.JobJobSkills)
                               .Include(j => j.Location)
                               .FirstOrDefault(j => j.JobID == model.JobID);

                if (jobInDb == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy bản ghi cần chỉnh sửa.");
                    return View(model);
                }

                try
                {
                    // Cập nhật thông tin cơ bản
                    jobInDb.JobTitle = model.JobTitle;
                    jobInDb.JobDescription = model.JobDescription;
                    jobInDb.JobRequirements = model.JobRequirements;
                    jobInDb.JobBenefits = model.JobBenefits;
                    jobInDb.JobCategoryID = model.JobCategoryID;
                    jobInDb.ExperienceID = model.ExperienceID;
                    jobInDb.SalaryID = model.SalaryID;
                    jobInDb.IsActive = model.IsActive;
                    jobInDb.IsNow = model.IsNow;
                    jobInDb.EndDate = model.EndDate;
                    jobInDb.ModifiedDate = DateTime.Now;
                    jobInDb.Alias = WebJob.Models.Common.Filter.FilterChar(model.JobTitle);

                    // Cập nhật Location
                    if (jobInDb.Location == null)
                    {
                        jobInDb.Location = new Location();
                    }
                    // Xử lý JobSkills
                    var existingSkillIds = jobInDb.JobJobSkills.Select(j => j.JobSkillId).ToList();
                    var selectedSkillIds = model.SelectedJobSkills ?? new int[0];

                    // Xóa các skill không được chọn
                    foreach (var existingSkill in jobInDb.JobJobSkills.ToList())
                    {
                        if (!selectedSkillIds.Contains(existingSkill.JobSkillId))
                        {
                            db.JobJobSkills.Remove(existingSkill);
                        }
                    }

                    // Thêm các skill mới
                    foreach (var skillId in selectedSkillIds)
                    {
                        if (!existingSkillIds.Contains(skillId))
                        {
                            jobInDb.JobJobSkills.Add(new Job_JobSkill
                            {
                                JobSkillId = skillId,
                                JobId = jobInDb.JobID
                            });
                        }
                    }
                    var companyUpdate = model.Company;

                    // Cập nhật thông tin công ty nếu có
                    if (jobInDb.Company != null)
                    {
                        if (!string.IsNullOrEmpty(model.CompanyName))
                            jobInDb.Company.CompanyName = model.CompanyName;

                        if (!string.IsNullOrEmpty(model.CompanyEmail))
                            jobInDb.Company.CompanyEmail = model.CompanyEmail;

                        if (!string.IsNullOrEmpty(model.CompanyDescription))
                            jobInDb.Company.CompanyDescription = model.CompanyDescription;

                        if (!string.IsNullOrEmpty(model.CompanyAddress))
                            jobInDb.Company.CompanyAddress = model.CompanyAddress;

                        jobInDb.Company.ModifiedDate = DateTime.Now;
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi khi lưu thay đổi: " + ex.Message);
                }
            }

            // Nếu có lỗi, load lại các dropdown
            ViewBag.Salary = new SelectList(db.Salaries.ToList(), "SalaryID", "SalaryRange", model.SalaryID);
            ViewBag.Experience = new SelectList(db.Experiences.ToList(), "ExperienceID", "ExperienceLevel", model.ExperienceID);
            ViewBag.JobCategory = new SelectList(db.JobCategories.ToList(), "JobCategoryID", "CategoryName", model.JobCategoryID);
            ViewBag.JobSkills = new MultiSelectList(db.JobSkills.ToList(), "Id", "JobSkillName", model.SelectedJobSkills);

            return View(model);
        }


        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                // Tìm công việc cần xóa
                var item = db.Jobs.Find(id);
                if (item != null)
                {
                    db.Jobs.Remove(item);
                    db.SaveChanges(); // Lưu thay đổi vào DB
                    return Json(new { success = true, message = "Xóa công việc thành công!" });
                }

                // Trả về lỗi nếu không tìm thấy công việc
                return Json(new { success = false, message = "Không tìm thấy công việc cần xóa." });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần) và trả về thông báo lỗi
                return Json(new { success = false });
            }
        }

    }
}