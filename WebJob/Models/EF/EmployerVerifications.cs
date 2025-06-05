using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;
using WebJob.Models.Enum;
namespace WebJob.Models.EF
{
    public class EmployerVerification
    {
        [Key]
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string BusinessLicenseNumber { get; set; }

        public string VerificationDocumentPath { get; set; }

        public string UserId { get; set; } 

        public int Status { get; set; } = (int)VerificationStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int PostRemain { get; set; } = 0; // Số bài đăng còn lại
        public int ValidityDays { get; set; } = 0; // Thời hạn sử dụng (ngày)
        public DateTime? ExpiryDate { get; set; } // Ngày hết hạn
        [ForeignKey("UserId")]
        public virtual ApplicationUser Users { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public virtual ICollection<EmployerVerificationProduct> EmployerVerificationProducts { get; set; }

        [NotMapped]
        public VerificationStatus StatusEnum
        {
            get => (VerificationStatus)Status;
            set => Status = (int)value;
        }
        public bool IsActive()
        {
            return ExpiryDate.HasValue && ExpiryDate.Value >= DateTime.Now && PostRemain > 0;
        }
    }

}
