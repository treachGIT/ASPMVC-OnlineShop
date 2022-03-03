using ASPMVCProjektSklep.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASPMVCProjektSklep.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Name == "Inne")
                {
                    return RedirectToAction("Index");
                }
                category.status = "active";

                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                if(category.Name == "Inne")
                {
                    return RedirectToAction("Index");
                }

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            if(category.Name == "Inne")
            {
                return RedirectToAction("Index");
            }

            Category others = db.Categories.Where(c => c.Name == "Inne").FirstOrDefault();
            if(others == null)
            {
                Category newCategory = new Category();
                newCategory.Name = "Inne";
                newCategory.status = "active";

                db.Categories.Add(newCategory);
                db.SaveChanges();

                others = newCategory;
            }

            foreach (var product in category.Products)
            {
                product.Category = others;
                others.Products.Add(product);

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Hide(int id)
        {
            Category category = db.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (category == null)
                return RedirectToAction("Index");

            foreach(Product p in category.Products)
            {
                p.status = "hidden";
            }

            category.status = "hidden";

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Activate(int id)
        {
            Category category = db.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (category == null)
                return RedirectToAction("Index");

            foreach (Product p in category.Products)
            {
                p.status = "active";
            }

            category.status = "active";

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public PartialViewResult MenuCategories()
        {
            IList<Category> categories;
            categories = db.Categories.Where(c => c.status == "active").ToList();   
            return PartialView("~/Views/Shared/_MenuCategories.cshtml", categories);
        }

    }
}