using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebJob.Models.EF;

namespace WebJob.Models.EF
{
    [Table("tb_CandidateProfileSkill")]
    public class CandidateProfileSkill : CommonAbstract
    {
        [Key]
        public int ID { get; set; }

        public int CandidateProfileID { get; set; }
        public int SkillID { get; set; }

        public virtual CandidateProfile CandidateProfile { get; set; }
        public virtual JobSkill Skill { get; set; }
    }
}
