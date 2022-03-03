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
    public class ImageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult ShowList(int id)
        {
            IList<Image> photos;

            Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
                return RedirectToAction("Index", "Product");

            photos = db.Images.Where(c => c.Product.Id == id).ToList();

            ViewBag.productName = product.Name;
            ViewBag.productID = product.Id;

            return View(photos);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create(int id)
        {
            Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
                return RedirectToAction("Index", "Product");

            ViewBag.ProductID = id;

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, [Bind(Include = "Id,Title,Source")] Image photo)
        {
            if (ModelState.IsValid)
            {
                Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();
                if (product == null)
                    return RedirectToAction("Index", "Product");

                photo.Product = product;

                if (photo.Title == "Main")
                {
                    ViewBag.ProductID = id;
                    foreach (Image p in product.Images)
                    {
                        if (p.Title == "Main")
                        {
                            ViewBag.titleError = "Obrazek ma już wyznaczone główne zdjęcie";
                            return View();
                        }
                    }
                }
                db.Images.Add(photo);
                db.SaveChanges();

                return RedirectToAction("ShowList", "Image", new { id = id });
            }
            return View(photo);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Image photo = db.Images.Where(c => c.Id == id).FirstOrDefault();
            if (photo == null)
                return RedirectToAction("Index", "Product");

            return View(photo);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Source")] Image photo)
        {   
            Image photop = db.Images.Where(c => c.Id == photo.Id).FirstOrDefault();
            if (photop == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = photop.Product;

            if (product == null)
                return RedirectToAction("Index", "Product");


            if (ModelState.IsValid)
            {
                if (photo.Title == "Main")
                {
                    foreach (Image p in product.Images)
                    {
                        if (p.Title == "Main" && p.Id != photo.Id)
                        {
                            ViewBag.titleError = "Obrazek ma już wyznaczone główne zdjęcie";
                            return View(photo);
                        }
                    }
                }

                photop.Title = photo.Title;
                photop.Source = photo.Source;
            }

            db.SaveChanges();

            return RedirectToAction("ShowList", "Image", new { id = product.Id });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Image photo = db.Images.Where(c => c.Id == id).FirstOrDefault();
            if (photo == null)
                return RedirectToAction("Index", "Product");

            long pID = -1;
            if (photo.Product != null)
                pID = photo.Product.Id;

            db.Images.Remove(photo);
            db.SaveChanges();

            if (pID != -1)
                return RedirectToAction("ShowList", "Image", new { id = pID });
            else
                return RedirectToAction("Index", "Product");
        }
    }
}