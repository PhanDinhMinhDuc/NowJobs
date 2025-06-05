using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebJob.Models.EF;
using WebJob.Models;

namespace WebJob.Models.EF
{
    [Table("tb_CandidateProfile")]
    public class CandidateProfile : CommonAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string UserId { get; set; } // Liên kết với ApplicationUser
        public string CVPath { get; set; } // Đường dẫn file CV
        public string AvatarPath { get; set; }
        public int? ExperienceID { get; set; }
        public int? PositionID { get; set; }
        public int? LocationID { get; set; }
        public int? JobCategoryID {  get; set; }
        public virtual ICollection<CandidateProfileSkill> CandidateProfileSkills { get; set; } = new HashSet<CandidateProfileSkill>();

        public virtual Experience Experience { get; set; }
        public virtual Position Position { get; set; }
        public virtual Location Location { get; set; }
        public virtual JobCategory JobCategory { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Application> Applications { get; set; } = new HashSet<Application>();
    }
}
