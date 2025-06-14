﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebJob.Filters;
using WebJob.Models;
using WebJob.Models.EF;

namespace WebJob.Areas.Admin.Controllers
{
    [CustomAuthorize(Area = "Admin", Roles = "Admin")]
    public class JobCategoryController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/JobCategory
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            var items = db.JobCategories.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(items);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(JobCategory model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                model.ModifiedDate = DateTime.Now;
                model.Alias = WebJob.Models.Common.Filter.FilterChar(model.CategoryName);
                db.JobCategories.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // Hiển thị form sửa
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var jobCategory = db.JobCategories.Find(id);
            if (jobCategory == null)
                return HttpNotFound();

            return View(jobCategory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JobCategory model)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = db.JobCategories.Find(model.JobCategoryID);
                if (existingCategory != null)
                {
                    // Cập nhật các trường dữ liệu
                    existingCategory.CategoryName = model.CategoryName;
                    existingCategory.Alias = WebJob.Models.Common.Filter.FilterChar(model.CategoryName);
                    existingCategory.Icon = model.Icon;
                    existingCategory.IsActive = model.IsActive;
                    existingCategory.ModifiedDate = DateTime.Now;
                    existingCategory.ModifiedBy = User.Identity.Name;
                    db.Entry(existingCategory).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy danh mục.");
                }
            }
            return View(model);
        }




        [HttpPost]
        public ActionResult Delete(int id)
        {

            var item = db.JobCategories.Find(id);
            if (item != null)
            {
                db.JobCategories.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });

            }
            return Json(new { success = false });
        }
    }
}