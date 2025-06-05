using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebJob
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // ================= ADMIN ROUTES =================
            routes.MapRoute(
                name: "DoanhThu",
                url: "doanh-thu",
                defaults: new { area = "Admin", controller = "Order", action = "RevenueStatistics" },
                namespaces: new[] { "WebJob.Areas.Admin.Controllers" }
            ).DataTokens["area"] = "Admin";

            // ================= EMPLOYER ROUTES =================
            routes.MapRoute(
                name: "EmployerHome",
                url: "Employer/Home",
                defaults: new { area = "Employer", controller = "Home", action = "Index" }
            ).DataTokens["area"] = "Employer";

            routes.MapRoute(
                name: "Packages",
                url: "Employer/Package",
                defaults: new { area = "Employer", controller = "Package", action = "Index" }
            ).DataTokens["area"] = "Employer";

            routes.MapRoute(
                name: "DangTin",
                url: "them-tin-tuyen-dung",
                defaults: new { area = "Employer", controller = "Jobs", action = "Add" },
                namespaces: new[] { "WebJob.Areas.Employer.Controllers" }
            ).DataTokens["area"] = "Employer";

            routes.MapRoute(
                name: "CVUNGTUYEN",
                url: "cv-ung-tuyen",
                defaults: new { area = "Employer", controller = "Application", action = "Index" },
                namespaces: new[] { "WebJob.Areas.Employer.Controllers" }
            ).DataTokens["area"] = "Employer";

            routes.MapRoute(
                name: "LienHeEmloyer",
                url: "lien-he-ho-tro",
                defaults: new { area = "Employer", controller = "Home", action = "Contact" },
                namespaces: new[] { "WebJob.Areas.Employer.Controllers" }
            ).DataTokens["area"] = "Employer";

            routes.MapRoute(
                name: "JobActive",
                url: "viec-lam-da-dang",
                defaults: new { area = "Employer", controller = "Jobs", action = "Index" },
                namespaces: new[] { "WebJob.Areas.Employer.Controllers" }
            ).DataTokens["area"] = "Employer";

            routes.MapRoute(
                name: "vnpay_return",
                url: "vnpay_return",
                defaults: new { controller = "Package", action = "VnpayReturn", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            ).DataTokens["area"] = "Employer";

            // ================= CANDIDATE/COMMON ROUTES =================
            // Home/Common
            routes.MapRoute(
                name: "Home",
                url: "trang-chu",
                defaults: new { controller = "Home", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );
            routes.MapRoute(
                name: "AppllyJob",
                url: "ung-tuyen/{jobId}",
                defaults: new { controller = "SaveJob", action = "AppllyJob", jobId = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );
            routes.MapRoute(
                name: "Contact",
                url: "lien-he",
                defaults: new { controller = "Contact", action = "Contact", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "Search", action = "Search" },
                namespaces: new[] { "WebJob.Controllers" }
            );

            // News
            routes.MapRoute(
                name: "News",
                url: "tin-tuc",
                defaults: new { controller = "News", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            routes.MapRoute(
                name: "DetailNews",
                url: "{alias}-n{id}",
                defaults: new { controller = "News", action = "Detail" },
                namespaces: new[] { "WebJob.Controllers" }
            );

            routes.MapRoute(
                name: "BaiViet",
                url: "post/{alias}",
                defaults: new { controller = "Articel", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            // Jobs
            routes.MapRoute(
                name: "CategoryJob",
                url: "viec-lam",
                defaults: new { controller = "Jobs", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            routes.MapRoute(
                name: "AllJob",
                url: "viec-lam",
                defaults: new { controller = "Jobs", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            routes.MapRoute(
                name: "CategoryJobleft",
                url: "{alias}-{id}",
                defaults: new { controller = "Jobs", action = "JobCategory", id = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            routes.MapRoute(
                name: "CategoryJobHome",
                url: "{alias}-{id}",
                defaults: new { controller = "Jobs", action = "View_itemsByCateId", id = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            routes.MapRoute(
                name: "DetailJob",
                url: "chi-tiet/{alias}-p{id}",
                defaults: new { controller = "Jobs", action = "Detail", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            // Job Application

            

            // Products/Packages
            routes.MapRoute(
                name: "Product",
                url: "goi",
                defaults: new { controller = "Products", action = "Index", alias = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );

            // Default Route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "WebJob.Controllers" }
            );
        }
    }
}