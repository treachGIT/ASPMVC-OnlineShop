using ASPMVCProjektSklep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVCProjektSklep.Controllers
{
    public class SaleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult ShowList(int id)
        {
            IList<Sale> sales;

            Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
                return RedirectToAction("Index", "Product");

            sales = db.Sales.Where(c => c.Product.Id == id).ToList();

            ViewBag.productName = product.Name;
            ViewBag.productID = product.Id;
            int[] saleQuantity = new int[sales.Count];

            int i = 0;
            foreach (Sale s in sales)
            {
                saleQuantity[i] = 0;
                foreach (OrderProduct op in product.OrderProducts)
                {
                    if (s.startDate <= op.Order.Date && s.endDate >= op.Order.Date)
                        saleQuantity[i] += op.Quantity;
                }
                i++;
            }
            ViewBag.saleQuantity = saleQuantity;

            return View(sales);
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
        public ActionResult Create(int id, [Bind(Include = "Id,Discount,startDate,endDate")] Sale sale)
        {
            Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();
            if (product == null)
                return RedirectToAction("Index", "Product");

            if (ModelState.IsValid)
            {
                sale.Product = product;

                if (sale.startDate >= sale.endDate)
                {
                    Product product1 = db.Products.Where(c => c.Id == id).FirstOrDefault();
                    if (product1 == null)
                        return RedirectToAction("Index", "Product");

                    ViewBag.ProductID = id;

                    ViewBag.endDateError = "Nieprawidłowa data zakończenia";

                    return View();
                }

                foreach (Sale s in product.Sales)
                {
                    if (IsBetween(sale.startDate, s.startDate, s.endDate) == false)
                    {
                        if (sale.startDate < s.startDate && sale.endDate < s.startDate)
                        {

                        }
                        else if (sale.startDate > s.endDate && sale.endDate > s.endDate)
                        {

                        }
                        else
                        {
                            Product product1 = db.Products.Where(c => c.Id == id).FirstOrDefault();
                            if (product1 == null)
                                return RedirectToAction("Index", "Product");

                            ViewBag.ProductID = id;

                            ViewBag.endDateError = "Isnieje inna przecena w tym przedziale dat";

                            return View();
                        }
                    }
                    else
                    {
                        Product product1 = db.Products.Where(c => c.Id == id).FirstOrDefault();
                        if (product1 == null)
                            return RedirectToAction("Index", "Product");

                        ViewBag.ProductID = id;

                        ViewBag.endDateError = "Isnieje inna przecena w tym przedziale dat";

                        return View();
                    }
                }
                db.Sales.Add(sale);
                db.SaveChanges();
            }
     
            return RedirectToAction("ShowList", "Sale", new { id = id });
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Sale sale = db.Sales.Where(c => c.Id == id).FirstOrDefault();
            if (sale == null)
                return RedirectToAction("Index", "Product");

            long pID = -1;
            if (sale.Product != null)
                pID = sale.Product.Id;

            db.Sales.Remove(sale);
            db.SaveChanges();

            if (pID != -1)
                return RedirectToAction("ShowList", "Sale", new { id = pID });
            else
                return RedirectToAction("Index", "Product");
        }

        public static bool IsBetween(DateTime time, DateTime startTime, DateTime endTime)
        {
            if (time.TimeOfDay == startTime.TimeOfDay) return true;
            if (time.TimeOfDay == endTime.TimeOfDay) return true;

            if (startTime.TimeOfDay <= endTime.TimeOfDay)
                return (time.TimeOfDay >= startTime.TimeOfDay && time.TimeOfDay <= endTime.TimeOfDay);
            else
                return !(time.TimeOfDay >= endTime.TimeOfDay && time.TimeOfDay <= startTime.TimeOfDay);
        }

    }
}