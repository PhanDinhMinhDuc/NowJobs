using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebJob.Filters;
using WebJob.Models.EF;
using WebJob.Models.ViewModels;

namespace WebJob.Controllers
{
    [CustomAuthorize(Roles = "Candidate")]
    public class ProfileController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult MyProfile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Thay thế bằng action và controller đăng nhập của bạn
            }
            var userId = User.Identity.GetUserId();
            var candidateProfile = _db.CandidateProfiles
                .Include(c => c.Experience)
                .Include(c => c.Position)
                .Include(c => c.Location)
                .Include(c => c.JobCategory)
                .Include(c => c.CandidateProfileSkills.Select(cps => cps.Skill))
                .FirstOrDefault(c => c.UserId == userId);

            if (candidateProfile == null)
            {
                // Nếu chưa có profile, tạo mới
                candidateProfile = new CandidateProfile { UserId = userId };
                _db.CandidateProfiles.Add(candidateProfile);
                _db.SaveChanges();
                return RedirectToAction("Edit");
            }

            return View(candidateProfile);
        }
        public ActionResult Edit()
        {
            var userId = User.Identity.GetUserId();
            var candidateProfile = _db.CandidateProfiles
                .Include(c => c.CandidateProfileSkills)
                .FirstOrDefault(c => c.UserId == userId);

            if (candidateProfile == null)
            {
                candidateProfile = new CandidateProfile { UserId = userId };
                _db.CandidateProfiles.Add(candidateProfile);
                _db.SaveChanges();
            }

            var model = new CandidateProfileViewModel
            {
                ID = candidateProfile.ID,
                CVPath = candidateProfile.CVPath,
                AvatarPath = candidateProfile.AvatarPath,
                ExperienceID = candidateProfile.ExperienceID,
                PositionID = candidateProfile.PositionID,
                LocationID = candidateProfile.LocationID,
                JobCategoryID = candidateProfile.JobCategoryID,
                SkillIDs = candidateProfile.CandidateProfileSkills?.Select(cps => cps.SkillID).ToList(),
                SelectedSkills = candidateProfile.CandidateProfileSkills?.Select(cps => cps.Skill).ToList(),
                Experiences = new SelectList(_db.Experiences.ToList(), "ExperienceID", "ExperienceLevel", candidateProfile.ExperienceID),
                Positions = new SelectList(_db.Positions.ToList(), "PositionID", "PositionName", candidateProfile.PositionID),
                Locations = new SelectList(_db.Locations.ToList(), "LocationID", "LocationName", candidateProfile.LocationID),
                JobCategories = new SelectList(_db.JobCategories.ToList(), "JobCategoryID", "CategoryName", candidateProfile.JobCategoryID),
                Skills = new MultiSelectList(_db.JobSkills.ToList(), "Id", "JobSkillName", candidateProfile.CandidateProfileSkills?.Select(cps => cps.SkillID))
            };

            return View(model);
        }

        // POST: CandidateProfile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CandidateProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var candidateProfile = _db.CandidateProfiles
                    .Include(c => c.CandidateProfileSkills)
                    .FirstOrDefault(c => c.UserId == userId);

                if (candidateProfile == null)
                {
                    return HttpNotFound();
                }

                // Xử lý upload CV
                if (model.CVFile != null && model.CVFile.ContentLength > 0)
                {
                    var uploadsFolder = Server.MapPath("~/Uploads/CVs");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.CVFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.CVFile.SaveAs(filePath);
                    if (!string.IsNullOrEmpty(candidateProfile.CVPath))
                    {
                        var oldFilePath = Server.MapPath(candidateProfile.CVPath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    candidateProfile.CVPath = "/Uploads/CVs/" + uniqueFileName;
                }

                // Xử lý upload ảnh đại diện
                if (model.AvatarFile != null && model.AvatarFile.ContentLength > 0)
                {
                    var uploadsFolder = Server.MapPath("~/Uploads/Avatars");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.AvatarFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.AvatarFile.SaveAs(filePath);

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(candidateProfile.AvatarPath))
                    {
                        var oldFilePath = Server.MapPath(candidateProfile.AvatarPath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    candidateProfile.AvatarPath = "/Uploads/Avatars/" + uniqueFileName;
                }

                // Cập nhật các thông tin khác
                candidateProfile.ExperienceID = model.ExperienceID;
                candidateProfile.PositionID = model.PositionID;
                candidateProfile.LocationID = model.LocationID;
                candidateProfile.JobCategoryID = model.JobCategoryID;

                // Xử lý kỹ năng
                if (model.SkillIDs != null)
                {
                    // Xóa các kỹ năng không còn được chọn
                    var existingSkillIds = candidateProfile.CandidateProfileSkills.Select(cps => cps.SkillID).ToList();
                    var selectedSkillIds = model.SkillIDs.ToList();

                    var toRemove = existingSkillIds.Except(selectedSkillIds).ToList();
                    var toAdd = selectedSkillIds.Except(existingSkillIds).ToList();

                    // Xóa các kỹ năng không còn được chọn
                    foreach (var skillId in toRemove)
                    {
                        var skillToRemove = candidateProfile.CandidateProfileSkills.FirstOrDefault(cps => cps.SkillID == skillId);
                        if (skillToRemove != null)
                        {
                            _db.CandidateProfileSkills.Remove(skillToRemove);
                        }
                    }

                    // Thêm các kỹ năng mới
                    foreach (var skillId in toAdd)
                    {
                        candidateProfile.CandidateProfileSkills.Add(new CandidateProfileSkill
                        {
                            CandidateProfileID = candidateProfile.ID,
                            SkillID = skillId,
                            CreatedBy = model.Email,
                            CreatedDate = DateTime.Now,
                        });
                    }
                }
                else
                {
                    // Nếu không có skill nào được chọn, xóa tất cả
                    _db.CandidateProfileSkills.RemoveRange(candidateProfile.CandidateProfileSkills);
                }

                _db.Entry(candidateProfile).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("MyProfile");
            }

            // Nếu ModelState không valid, load lại các dropdown
            model.Experiences = new SelectList(_db.Experiences.ToList(), "ID", "ExperienceLevel", model.ExperienceID);
            model.Positions = new SelectList(_db.Positions.ToList(), "ID", "PositionName", model.PositionID);
            //model.Skills = new MultiSelectList(_db.JobSkills.ToList(), "Id", "JobSkillName", model.SelectedJobSkills);
            model.JobCategories = new SelectList(_db.JobCategories.ToList(), "JobCategoryID", "CategoryName");
            model.Experiences = new SelectList(_db.Experiences.ToList(), "ExperienceID", "ExperienceLevel");
            model.Locations = new SelectList(_db.Locations.ToList(), "LocationID", "LocationName");

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}