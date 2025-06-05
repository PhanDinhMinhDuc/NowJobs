using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJob.Models.ViewModels
{
    public class MyAppliedJobViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FeebBack { get; set; } // Nhận xét từ bảng Applicant
        public int ApplicationStatus { get; set; }
        public string CompanyLogo { get; set; } // Thêm trường hình ảnh
        public string SalaryRange { get; set; } // Thêm trường lương
        public string PositionName { get; set; } // Thêm trường vị trí
        public DateTime? EndDate { get; set; } // Thêm trường hạn nộp
        public string Alias { get; set; } // Thêm trường alias cho URL
        public string LocationName { get; set; }
    }

}