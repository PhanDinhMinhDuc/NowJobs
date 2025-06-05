using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;
using WebJob.Models.EF;

namespace WebJob.Models.ViewModels
{
    public class CandidateProfileViewModel
    {
        public int ID { get; set; }

        [Display(Name = "CV")]
        public HttpPostedFileBase CVFile { get; set; }
        public string CVPath { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public HttpPostedFileBase AvatarFile { get; set; }
        public string AvatarPath { get; set; }

        [Display(Name = "Kinh nghiệm")]
        public int? ExperienceID { get; set; }

        [Display(Name = "Vị trí")]
        public int? PositionID { get; set; }
        [Display(Name = "Địa điểm")]
        public int? LocationID { get; set; }

        [Display(Name = "Ngành nghề")]
        public int? JobCategoryID { get; set; }

        [Display(Name = "Kỹ năng")]
        public List<int> SkillIDs { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public SelectList Experiences { get; set; }
        public SelectList Positions { get; set; }
        public SelectList Locations { get; set; }
        public SelectList JobCategories { get; set; }
        public MultiSelectList Skills { get; set; }
        public List<JobSkill> SelectedSkills { get; set; }
    }
}