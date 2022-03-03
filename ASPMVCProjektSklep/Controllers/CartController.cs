using ASPMVCProjektSklep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;


namespace ASPMVCProjektSklep.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Show()
        {
            int cartId = Convert.ToInt32(Request.Cookies["cartId"].Value);

            Cart cart = db.Carts.Where(b => b.Id == cartId).FirstOrDefault();
            if (cart == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<int> productIDs = new List<int>();
            List<string> productNames = new List<string>();
            List<int> quantities = new List<int>();
            List<float> prices = new List<float>();
            List<string> photos = new List<string>();
            float amount = 0;

            foreach (var item in cart.CartProducts)
            {
                if (item.Product.status == "active")
                {
                    productIDs.Add(item.Product.Id);
                    productNames.Add(item.Product.Name);
                    quantities.Add(item.Quantity);

                    if (item.Product.Images.Count == 0)
                    {
                        photos.Add("~/Images/noimage.png");
                    }
                    else
                    {
                        string srccc = "";
                        foreach (Image p in item.Product.Images)
                        {
                            if (p.Title == "Main")
                                srccc = p.Source;
                        }

                        if (srccc == "")
                            srccc = item.Product.Images.FirstOrDefault().Source;

                        photos.Add(srccc);
                    }

                    float sale = -1;
                    foreach (Sale s in item.Product.Sales)
                    {
                        if (s.startDate <= DateTime.Now && s.endDate >= DateTime.Now)
                        {
                            float temp = (float)(100 - s.Discount) / 100;
                            float tempsale = item.Product.Price * temp;
                            float rounded = (float)(Math.Round((double)tempsale, 2));
                            string temppp = rounded.ToString("0.00");
                            sale = float.Parse(temppp);
                        }
                    }

                    if (sale == -1)
                    {
                        float tempPrice = item.Product.Price * item.Quantity;
                        prices.Add(tempPrice);
                        amount += tempPrice;
                    }
                    else
                    {
                        float tempPrice = sale * item.Quantity;
                        prices.Add(tempPrice);
                        amount += tempPrice;
                    }
                }
                else
                {
                    db.CartProducts.Remove(item);
                    db.SaveChanges();                 
                }
            }

            ViewBag.productIDs = productIDs;
            ViewBag.productNames = productNames;
            ViewBag.quantities = quantities;
            ViewBag.prices = prices;
            ViewBag.photos = photos;
            ViewBag.amount = amount.ToString("0.00");

            return View();
        }

        public ActionResult AddToCart(int id)
        {
            int cartId = Convert.ToInt32(Request.Cookies["cartId"].Value);

            Cart cart = db.Carts.Where(b => b.Id == cartId).FirstOrDefault();
            Product product = db.Products.Where(b => b.Id == id).FirstOrDefault();

            if (cart == null || product == null)
                return RedirectToAction("Index", "Home");

            CartProduct temp = db.CartProducts.Where(b => b.Product.Id == product.Id && b.Cart.Id == cart.Id).FirstOrDefault();
            if (temp != null)
            {
                temp.Quantity += 1;
                db.SaveChanges();
            }
            else
            {
                CartProduct cp = new CartProduct();
                cp.Product = product;
                cp.Cart = cart;
                cp.Quantity = 1;
                cart.CartProducts.Add(cp);
                product.CartProducts.Add(cp);

                db.SaveChanges();
            }      
            return RedirectToAction("Show", "Cart");
        }

        public ActionResult MinusFromCart(int id)
        {
            int cartId = Convert.ToInt32(Request.Cookies["cartId"].Value);

            Cart cart = db.Carts.Where(b => b.Id == cartId).FirstOrDefault();
            Product product = db.Products.Where(b => b.Id == id).FirstOrDefault();

            if (cart == null || product == null)
                return RedirectToAction("Index", "Home");

            CartProduct temp = db.CartProducts.Where(b => b.Product.Id == product.Id && b.Cart.Id == cart.Id).FirstOrDefault();
            if (temp != null)
            {
                if (temp.Quantity == 1)
                {
                    RemoveFromCart(temp.Product.Id);
                }
                else
                {
                    temp.Quantity -= 1;
                }               
            }
            db.SaveChanges();
       
            return RedirectToAction("Show", "Cart");
        }


        public ActionResult RemoveFromCart(int id)
        {
            int cartId = Convert.ToInt32(Request.Cookies["cartId"].Value);

            Cart cart = db.Carts.Where(b => b.Id == cartId).FirstOrDefault();
            Product product = db.Products.Where(b => b.Id == id).FirstOrDefault();

            if (cart == null || product == null)
                return RedirectToAction("Index", "Home");

            CartProduct temp = db.CartProducts.Where(b => b.Product.Id == product.Id && b.Cart.Id == cart.Id).FirstOrDefault();
            if (temp == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                db.CartProducts.Remove(temp);
                db.SaveChanges();
                return RedirectToAction("Show", "Cart");
            }
        }
    
        public PartialViewResult CartDropDown()
        {
            if (Request.Cookies["cartId"] == null)
            {
                ViewBag.CartMessage = "Koszyk jest pusty";

                Cart newCart = new Cart();
                newCart.CreationDate = DateTime.Now;
                db.Carts.Add(newCart);
                db.SaveChanges();

                HttpCookie cookie = new HttpCookie("cartId");
                cookie.Value = newCart.Id.ToString();
                cookie.Expires = DateTime.Now.AddMinutes(120);
                Response.Cookies.Add(cookie);

                return PartialView("~/Views/Cart/CartDropdown.cshtml", null);
            }
            else
            {
                IList<Product> products = new List<Product>();
                int Id = Convert.ToInt32(Request.Cookies["cartId"].Value);

                Cart cart = db.Carts.Where(b => b.Id == Id).FirstOrDefault();
                if (cart != null)
                {
                    if (cart.CartProducts.Count != 0)
                    {
                        int NumberOfItems = 0;
                        foreach (var item in cart.CartProducts)
                        {
                            NumberOfItems += item.Quantity;
                        }

                        ViewBag.NumberOfItems = NumberOfItems;
                    }
                    else
                    {
                        ViewBag.NumberOfItems = 0;
                    }

                    return PartialView("~/Views/Cart/CartDropdown.cshtml");
                }
                else
                {
                    Cart newCart = new Cart();
                    newCart.CreationDate = DateTime.Now;
                    db.Carts.Add(newCart);
                    db.SaveChanges();

                    HttpCookie cookie = new HttpCookie("cartId");
                    cookie.Value = newCart.Id.ToString();
                    cookie.Expires = DateTime.Now.AddMinutes(120);
                    Response.Cookies.Add(cookie);
                    ViewBag.NumberOfItems = 0;

                    return PartialView("~/Views/Cart/CartDropdown.cshtml");
                }
            }
        }
    }
}