using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WebJob.Models.EF;

namespace WebJob.Models.ViewModels
{
    public class JobViewModel
    {
        public int JobID { get; set; }

        [Required]
        [StringLength(255)]
        public string JobTitle { get; set; }

        public int? CompanyID { get; set; }
        public int? SalaryID { get; set; }
        public int? PositionID { get; set; }
        public int? ExperienceID { get; set; }
        public int? LocationID { get; set; }
        public int? IndustryID { get; set; }
        public int? LevelID { get; set; }
        public int? JobCategoryID { get; set; }
        public int ViewCount { get; set; }
        public string Alias { get; set; }
        [AllowHtml]
        public string JobDescription { get; set; }
        [AllowHtml]
        public string JobBenefits { get; set; }
        [AllowHtml]
        public string JobRequirements { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsNow { get; set; } = true;
        // Thêm trường UserId để lưu trữ ID của người đăng công việc
        public string UserId { get; set; }
        public DateTime EndDate { get; set; }
        public virtual Company Company { get; set; }
        public virtual Salary Salary { get; set; }
        public virtual Position Position { get; set; }
        public virtual Experience Experience { get; set; }
        public virtual Location Location { get; set; }
        public virtual Level Level { get; set; }
        public virtual JobCategory JobCategory { get; set; }
        public int[] SelectedJobSkills { get; set; }
        public MultiSelectList JobSkillsList { get; set; }

        public SelectList JobCategories { get; set; }
        public SelectList Experiences { get; set; }
        public SelectList Salaries { get; set; }
        public SelectList Locations { get; set; }
        public SelectList Positions { get; set; }   
        public List<CompanyImage> CompanyImages { get; set; } = new List<CompanyImage>();
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyAddress { get; set; }
    }
}
