using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebJob.Models;
using WebJob.Models.EF;
using PagedList;
using System.Data.Entity;
using System.IO;
using WebJob.Filters;

namespace WebJob.Areas.Admin.Controllers
{
    [CustomAuthorize(Area = "Admin", Roles = "Admin")]
    public class CompanyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Company
        public ActionResult Index(int? page, string search = null)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            // Truy vấn dữ liệu
            var companiesQuery = db.Companies
                                   .Include(c => c.CompanyImages) // Kết nối bảng hình ảnh công ty
                                   .AsQueryable();

            // Lọc theo từ khóa tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
            {
                companiesQuery = companiesQuery.Where(c => c.CompanyName.Contains(search));
            }

            // Loại bỏ tên công ty trùng lặp
            var filteredCompanies = companiesQuery
                                    .GroupBy(c => c.CompanyName)
                                    .Select(g => g.FirstOrDefault())
                                    .OrderByDescending(c => c.CompanyID);

            // Phân trang
            var items = filteredCompanies.ToPagedList(pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            ViewBag.Search = search; // Lưu từ khóa tìm kiếm để hiển thị lại trong form

            return View(items);
        }
        [HttpGet]
        public ActionResult Add()
        {
            return PartialView("_AddEditCompany", new Company());
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var company = db.Companies.Include(c => c.CompanyImages).FirstOrDefault(c => c.CompanyID == id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return PartialView("_AddEditCompany", company);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Company model, List<HttpPostedFileBase> images)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.CompanyID == 0) // Add new
                    {
                        model.Alias = WebJob.Models.Common.Filter.FilterChar(model.CompanyName);
                        db.Companies.Add(model);
                        db.SaveChanges();

                        // Save images
                        SaveCompanyImages(model.CompanyID, images);
                    }
                    else // Update
                    {
                        var existingCompany = db.Companies.Find(model.CompanyID);
                        if (existingCompany != null)
                        {
                            existingCompany.CompanyName = model.CompanyName;
                            existingCompany.CompanyDescription = model.CompanyDescription;
                            existingCompany.CompanyEmail = model.CompanyEmail;
                            existingCompany.CompanyAddress = model.CompanyAddress;
                            existingCompany.Alias = WebJob.Models.Common.Filter.FilterChar(model.CompanyName);
                            existingCompany.IsActive = model.IsActive;

                            db.Entry(existingCompany).State = EntityState.Modified;
                            db.SaveChanges();

                            // Save images
                            SaveCompanyImages(model.CompanyID, images);
                        }
                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return PartialView("_AddEditCompany", model);
        }
        private void SaveCompanyImages(int companyId, List<HttpPostedFileBase> images)
        {
            if (images != null && images.Count > 0)
            {
                foreach (var file in images)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(Server.MapPath("~/Uploads/Companies/"), fileName);
                        file.SaveAs(path);

                        var companyImage = new CompanyImage
                        {
                            CompanyID = companyId,
                            ImageURL = "/Uploads/Companies/" + fileName,
                            IsDefault = false // You can add logic to set default image
                        };

                        db.CompanyImages.Add(companyImage);
                    }
                }
                db.SaveChanges();
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var company = db.Companies.Find(id);
            if (company == null)
            {
                return Json(new { success = false, message = "Company not found." });
            }

            try
            {
                // Delete associated images first
                var images = db.CompanyImages.Where(ci => ci.CompanyID == id).ToList();
                foreach (var image in images)
                {
                    // Delete physical file
                    string imagePath = Server.MapPath(image.ImageURL);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    db.CompanyImages.Remove(image);
                }

                db.Companies.Remove(company);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult DeleteAll(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json(new { success = false, message = "No companies selected." });
            }

            var idArray = ids.Split(',').Select(int.Parse).ToArray();
            try
            {
                foreach (var id in idArray)
                {
                    var company = db.Companies.Find(id);
                    if (company != null)
                    {
                        // Delete associated images
                        var images = db.CompanyImages.Where(ci => ci.CompanyID == id).ToList();
                        foreach (var image in images)
                        {
                            // Delete physical file
                            string imagePath = Server.MapPath(image.ImageURL);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                            db.CompanyImages.Remove(image);
                        }

                        db.Companies.Remove(company);
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var company = db.Companies.Find(id);
            if (company == null)
            {
                return Json(new { success = false, message = "Company not found." });
            }

            company.IsActive = !company.IsActive;
            db.SaveChanges();

            return Json(new { success = true, isActive = company.IsActive });
        }
        [HttpPost]
        public ActionResult DeleteImage(int id)
        {
            var image = db.CompanyImages.Find(id);
            if (image == null)
            {
                return Json(new { success = false, message = "Image not found." });
            }

            try
            {
                // Delete physical file
                string imagePath = Server.MapPath(image.ImageURL);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                db.CompanyImages.Remove(image);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SetDefaultImage(int companyId, int imageId)
        {
            try
            {
                // Reset all images for this company to non-default
                var companyImages = db.CompanyImages.Where(ci => ci.CompanyID == companyId).ToList();
                foreach (var img in companyImages)
                {
                    img.IsDefault = false;
                    db.Entry(img).State = EntityState.Modified;
                }

                // Set the selected image as default
                var defaultImage = companyImages.FirstOrDefault(ci => ci.CompanyImageID == imageId);
                if (defaultImage != null)
                {
                    defaultImage.IsDefault = true;
                    db.Entry(defaultImage).State = EntityState.Modified;
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}