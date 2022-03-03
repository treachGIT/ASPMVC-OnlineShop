using ASPMVCProjektSklep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ASPMVCProjektSklep.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            IList<Order> orders;

            orders = db.Orders.ToList(); 


            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ChangeStatusToInProgress(int id)
        {
            Order order = db.Orders.Where(c => c.Id == id).FirstOrDefault();
            if (order == null)
                return RedirectToAction("Index", "Order");

            order.status = "w trakcie realizacji";

            db.SaveChanges();

            return RedirectToAction("Index", "Order");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ChangeStatusToRealized(int id)
        {
            Order order = db.Orders.Where(c => c.Id == id).FirstOrDefault();
            if (order == null)
                return RedirectToAction("Index", "Order");

            order.status = "zrealizowane";

            db.SaveChanges();

            return RedirectToAction("Index", "Order");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Order order = db.Orders.Where(c => c.Id == id).FirstOrDefault();
            if (order == null)
                return RedirectToAction("Index", "Order");

            if (order.status == "zrealizowane")
                return RedirectToAction("Index", "Order");

            foreach (OrderProduct op in order.OrderProducts)
            {
                db.OrderProducts.Remove(op);
                db.SaveChanges();      
            }

            Order order1 = db.Orders.Where(c => c.Id == id).FirstOrDefault();
            if (order1 == null)
                return RedirectToAction("Index", "Order");

            db.Orders.Remove(order1);
            db.SaveChanges();

            return RedirectToAction("Index", "Order");
        }

        [Authorize]
        public ActionResult Create()
        {
            int cartId = Convert.ToInt32(Request.Cookies["cartId"].Value);

            var userId = User.Identity.GetUserId();                   

            List<Adress> adresses = db.Adresses.Where(b => b.User.Id == userId ).ToList();   
            if (adresses.Count == 0)
            {
                ViewBag.adressMessage = "Żeby złożyc zamówienie musisz posiadać adres dodany do twojego konta";
                return View();
            }

            Cart cart = db.Carts.Where(b => b.Id == cartId).FirstOrDefault();
            if (cart == null)
                return RedirectToAction("Index", "Home");

            List<long> productIDs = new List<long>();
            List<string> productNames = new List<string>();
            List<float> prices = new List<float>();
            List<int> quantities = new List<int>();
            float amount = 0;

            if (cart.CartProducts.Count == 0)
            {
                return RedirectToAction("Show", "Cart");
            }

            foreach (var item in cart.CartProducts)
            {
                if (item.Product.status == "active")
                {
                    productIDs.Add(item.Product.Id);
                    productNames.Add(item.Product.Name);

                    float sale = -1;
                    foreach (Sale s in item.Product.Sales)
                    {
                        if (s.startDate <= DateTime.Now && s.endDate >= DateTime.Now)
                        {
                            float temp1 = (float)(100 - s.Discount) / 100;
                            float tempsale = item.Product.Price * temp1;
                            float rounded = (float)(Math.Round((double)tempsale, 2));
                            string temppp = rounded.ToString("0.00");
                            sale = float.Parse(temppp);
                        }
                    }

                    if (sale == -1)
                    {
                        float temp = item.Quantity * item.Product.Price;
                        prices.Add(temp);
                        amount += temp;
                        quantities.Add(item.Quantity);
                    }
                    else
                    {
                        float temp = item.Quantity * sale;
                        prices.Add(temp);
                        amount += temp;
                        quantities.Add(item.Quantity);
                    }
                }
                else
                {
                    db.CartProducts.Remove(item);
                    db.SaveChanges();
                }
            }

            List<SelectListItem> items = new List<SelectListItem>();


            foreach (var adress in adresses)
            {
                items.Add(new SelectListItem
                {
                    Text = adress.city + ", " + adress.street
                    + ", " + adress.building + ", " + adress.apartment + ", " + adress.postalCode
                    ,
                    Value = adress.Id.ToString()
                });
            }

            ViewBag.adresses = items;
           // ViewBag.userName = User.Identity.Name;
            ViewBag.userEmail = User.Identity.Name;

            ViewBag.productIDs = productIDs;
            ViewBag.productNames = productNames;
            ViewBag.quantities = quantities;
            ViewBag.prices = prices;
            ViewBag.amount = amount.ToString("0.00");

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            int cartId = Convert.ToInt32(Request.Cookies["cartId"].Value);
            int adressId = Int32.Parse(collection["Adress"]);

            ICollection<CartProduct> cartProducts = new List<CartProduct>();

            Cart cart = db.Carts.Where(b => b.Id == cartId).FirstOrDefault();

            foreach (var item in cart.CartProducts)
            {
                cartProducts.Add(item);
            }

            Adress adress = db.Adresses.Where(b => b.Id == adressId).FirstOrDefault();
            if (adress == null)
            {
                ViewBag.adressMessage = "Żeby złożyc zamówienie musisz posiadać adres dodany do twojego konta";
                return View();
            }

            var userId = User.Identity.GetUserId();
            ApplicationUser appUser = db.Users.FirstOrDefault(x => x.Id == userId);

            Order order = new Order();
            order.Date = DateTime.Now;
            order.Adress = adress;
            order.User = appUser;
            order.status = "nowe";

            db.Orders.Add(order);

            db.SaveChanges();


            float amount = 0;

            if (cartProducts.Count == 0)
            {
                db.Orders.Remove(order);
                return RedirectToAction("Show", "Cart");
            }

            foreach (var item in cartProducts)
            {
                Product product = db.Products.Where(b => b.Id == item.Product.Id).FirstOrDefault();

                if (product != null && product.status == "active")
                {
                    OrderProduct op = new OrderProduct();
                    op.Product = product;
                    op.Quantity = item.Quantity;
                    op.Order = order;

                    float sale = -1;
                    foreach (Sale s in op.Product.Sales)
                    {
                        if (s.startDate <= DateTime.Now && s.endDate >= DateTime.Now)
                        {
                            float temp1 = (float)(100 - s.Discount) / 100;
                            float tempsale = op.Product.Price * temp1;
                            float rounded = (float)(Math.Round((double)tempsale, 2));
                            string temppp = rounded.ToString("0.00");
                            sale = float.Parse(temppp);
                        }
                    }

                    if (sale == -1)
                    {
                        float temp = item.Quantity * product.Price;
                        amount += temp;
                        op.Price = temp;
                    }
                    else
                    {
                        float temp = item.Quantity * sale;
                        amount += temp;
                        op.Price = temp;
                    }

                    db.OrderProducts.Add(op);
                    db.SaveChanges();
                   // order.OrderProducts.Add(op);
                   // product.OrderProducts.Add(op);
                }
                else
                {
                    db.CartProducts.Remove(item);
                    db.SaveChanges();
                }
            }

            if (order.OrderProducts.Count == 0)
            {
                db.Orders.Remove(order);
                db.SaveChanges();
                return RedirectToAction("Show", "Cart");
            }

            foreach (var item in cartProducts)
            {
                db.CartProducts.Remove(item);
                db.SaveChanges();
            }

            float roundedamount = (float)(Math.Round((double)float.Parse(collection["amount"].ToString()), 2));
            ViewBag.amount = roundedamount;

            return RedirectToAction("OrderHistory", "Order");
        }

        [Authorize]
        public ActionResult OrderHistory()
        {
            var userId = User.Identity.GetUserId();

            ICollection<Order> orders = db.Orders.Where(b => b.User.Id == userId).ToList();

            return View(orders);        
        }

        [Authorize]
        public ActionResult OrderDetails(int id)
        {
            Order order = db.Orders.Where(b => b.Id== id).FirstOrDefault();
            if(order == null)
            {
                RedirectToAction("Index", "Manage");
            }

            return View(order);
        }

        [Authorize]
        public ActionResult RestartOrderProducts(int id)
        {
            Order order = db.Orders.Where(b => b.Id == id).FirstOrDefault();
            if (order == null)
            {
                RedirectToAction("Index", "Manage");
            }

            int cartId = Convert.ToInt32(Request.Cookies["cartId"].Value);
            Cart cart = db.Carts.Where(b => b.Id == cartId).FirstOrDefault();
            if (cart == null)
            {
                RedirectToAction("Index", "Manage");
            }

            foreach (CartProduct cp in cart.CartProducts)
            {
                db.CartProducts.Remove(cp);
                db.SaveChanges();
            }

            foreach (OrderProduct op in order.OrderProducts)
            {
                if(op.Product != null && op.Product.status == "active")
                {
                    CartProduct cp = new CartProduct();
                    cp.Cart = cart;
                    cp.Product = op.Product;
                    cp.Quantity = op.Quantity;

                    db.CartProducts.Add(cp);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Show","Cart");
        }
    }
}