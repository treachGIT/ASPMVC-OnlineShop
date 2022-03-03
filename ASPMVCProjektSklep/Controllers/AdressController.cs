using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASPMVCProjektSklep.Models;
using Microsoft.AspNet.Identity;

namespace ASPMVCProjektSklep.Controllers
{
    public class AdressController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Adresses
        public ActionResult Index()
        {
            return View(db.Adresses.ToList());
        }

        // GET: Adresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adress adress = db.Adresses.Find(id);
            if (adress == null)
            {
                return HttpNotFound();
            }
            return View(adress);
        }

        // GET: Adresses/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,city,street,building,apartment,postalCode")] Adress adress)
        {

            var userId = User.Identity.GetUserId();
            ApplicationUser appUser = db.Users.FirstOrDefault(x => x.Id == userId);
            adress.User = appUser;

            if (ModelState.IsValid)
            {
                db.Adresses.Add(adress);
                db.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }

            return View(adress);
        }

        // GET: Adresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adress adress = db.Adresses.Find(id);
            if (adress == null)
            {
                return HttpNotFound();
            }
            return View(adress);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,city,street,building,apartment,postalCode")] Adress adress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adress);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            Adress adress = db.Adresses.Find(id);

            if (adress.Orders.Count > 0)
            {
                return RedirectToAction("Index", "Manage");
            }

            db.Adresses.Remove(adress);
            db.SaveChanges();
            return RedirectToAction("Index", "Manage");
        }

        [Authorize]
        public PartialViewResult ShowAdresses()
        {
            ICollection<Adress> adresses = new List<Adress>();

            var userId = User.Identity.GetUserId();

            adresses = db.Adresses.Where(b => b.User.Id == userId).ToList();

            return PartialView("~/Views/Adress/ShowAdresses.cshtml", adresses);
       
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
