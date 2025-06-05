using System.Web;
using System.Web.Mvc;

namespace WebJob.Filters // sửa lại theo namespace của bạn
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string Area { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var user = filterContext.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                // Chưa đăng nhập → chuyển đúng login của Area
                var loginUrl = string.IsNullOrEmpty(Area)
                    ? "/Account/Login" // Candidate
                    : $"/{Area}/Account/Login"; // Admin hoặc Employer

                filterContext.Result = new RedirectResult(loginUrl);
                return;
            }

            // Nếu là Admin → luôn cho phép truy cập
            if (user.IsInRole("Admin"))
            {
                return;
            }

            // Nếu Area là Admin → chỉ Admin được vào
            if (Area == "Admin" && !user.IsInRole("Admin"))
            {
                filterContext.Result = new RedirectResult("Admin/Account/Login");
                return;
            }

            // Nếu Area là Employer → chỉ Employer được vào
            if (Area == "Employer" && !user.IsInRole("Employer"))
            {
                filterContext.Result = new RedirectResult("/Employer/Account/Login");
                return;
            }

            // Nếu không thuộc Area nào (tức là Candidate) → chỉ Candidate được vào
            if (string.IsNullOrEmpty(Area) && !user.IsInRole("Candidate"))
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            // Nếu qua hết → cho phép truy cập
        }
    }
}
