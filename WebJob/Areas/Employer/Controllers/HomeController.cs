using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebJob.Filters;
using WebJob.Models.Enum;
using WebJob.Models.ViewModels;


namespace WebJob.Areas.Employer.Controllers
{
    [CustomAuthorize(Area = "Employer", Roles = "Employer")]
    public class HomeController : Controller
    {
        // GET: Employer/Home
        private ApplicationDbContext _context = new ApplicationDbContext();
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var model = new EmployerHomeViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var employer = _context.employerVerifications.FirstOrDefault(e => e.UserId == userId);
                model.IsVerified = employer?.Status == (int)VerificationStatus.Approved ? true : false;
                model.HasActivePackage = employer?.ExpiryDate > DateTime.Now;
            }

            return View(model);
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
    
}