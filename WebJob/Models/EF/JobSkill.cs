using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebJob.Models.EF
{
    [Table("tb_JobSkill")]
    public class JobSkill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string JobSkillName { get; set; }
        public virtual ICollection<Job_JobSkill> JobJobSkills { get; set; } = new HashSet<Job_JobSkill>();

    }

}