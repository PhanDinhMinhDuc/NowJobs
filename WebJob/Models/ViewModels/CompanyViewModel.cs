using System.Collections.Generic;
using System.Web;
using WebJob.Models.EF;

namespace WebJob.Models.ViewModels
{
    public class CompanyViewModel
    {
        public Company Company { get; set; }
        public List<CompanyImage> CompanyImages { get; set; } = new List<CompanyImage>();
        public HttpPostedFileBase[] ImageUploads { get; set; }
    }
}
