using ASPMVCProjektSklep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVCProjektSklep.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            IList<Product> products;
            IList<Sale> sales;
            IList<Product> news;

            products = db.Products.Where(p => p.status == "active").ToList();
            products = products.OrderByDescending(c => c.OrderProducts.Count).ToList();

            List<int> productsP = new List<int>();
            for (int i = 0; i < products.Count; i++)
            {
                if (i < 4)
                    productsP.Add(products[i].Id);
            }
            ViewBag.mostPopular = productsP;

            sales = db.Sales.Where(s => s.startDate <= DateTime.Now && s.endDate >= DateTime.Now && s.Product.status == "active").ToList();
            sales = sales.OrderByDescending(c => c.Discount).ToList();

            List<int> productsS = new List<int>();
            for (int i = 0; i < sales.Count; i++)
            {
                if (i < 4)
                    productsS.Add(sales[i].Product.Id);
            }
            ViewBag.bestSale = productsS;

            news = db.Products.Where(p => p.status == "active").ToList();
            news = news.OrderByDescending(c => c.creationDate).ToList();
            List<long> productsN = new List<long>();
            for (int i = 0; i < news.Count; i++)
            {
                if (i < 4)
                    productsN.Add(news[i].Id);
            }
            ViewBag.news = productsN;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}