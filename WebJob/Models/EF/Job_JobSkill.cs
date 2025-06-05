using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebJob.Models.EF
{
    [Table("tb_Job_JobSkill")]
    public class Job_JobSkill
    {
        [Key, Column(Order = 0)]
        public int JobId { get; set; }

        [Key, Column(Order = 1)]
        public int JobSkillId { get; set; }

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        [ForeignKey("JobSkillId")]
        public virtual JobSkill JobSkill { get; set; }
    }
}