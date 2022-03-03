using ASPMVCProjektSklep.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ASPMVCProjektSklep.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            IList<Product> products;
            products = db.Products.ToList();
      
            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            IList<Category> categories = db.Categories.ToList();

            foreach (var category in categories)
            {
                items.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
            }

            ViewBag.categories = items;

            if (items.Count == 0)
                return RedirectToAction("Index");

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            if(validateProduct(collection["Name"].ToString(), collection["Price"], collection["Description"]) == false)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                IList<Category> categories = db.Categories.ToList();
                foreach (var category in categories)
                {
                    items.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
                }
                ViewBag.categories = items;
                if (items.Count == 0)
                    return RedirectToAction("Index");

                return View();
            }

            Product product = new Product();

            if (ModelState.IsValid)
            {
                product.Name = collection["Name"].ToString();
                product.Price = float.Parse(collection["Price"]);
                product.Description = collection["Description"].ToString();
                int catID = Int32.Parse(collection["Category"]);

                Category cat = db.Categories.Where(c => c.Id == catID).FirstOrDefault();
                if(cat == null)
                {
                    return HttpNotFound();
                }

                product.Category = cat;
                product.creationDate = DateTime.Now;
                product.status = "active";

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> items = new List<SelectListItem>();
            Category pCategory = product.Category;

            var categories = db.Categories.ToList();

            items.Add(new SelectListItem { Text = pCategory.Name, Value = pCategory.Id.ToString() });

            foreach (var category in categories)
            {
                if (category.Id != pCategory.Id)
                    items.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
            }

            ViewBag.categories = items;

            if (items.Count == 0)
                return RedirectToAction("Index");

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection collection)
        {
            int Id = Int32.Parse(collection["Id"]);
            Product product = db.Products.Where(c => c.Id == Id).FirstOrDefault();
            if(product == null)
            {
                return HttpNotFound();
            }

            if (validateProduct(collection["Name"].ToString(), collection["Price"], collection["Description"]) == false)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                Category pCategory = product.Category;
                var categories = db.Categories.ToList();
                items.Add(new SelectListItem { Text = pCategory.Name, Value = pCategory.Id.ToString() });
                foreach (var category in categories)
                {
                    if (category.Id != pCategory.Id)
                        items.Add(new SelectListItem { Text = category.Name, Value = category.Id.ToString() });
                }
                ViewBag.categories = items;
                if (items.Count == 0)
                    return RedirectToAction("Index");

                return View(product);
            }

            if (ModelState.IsValid)
            {
                product.Name = collection["Name"].ToString();
                product.Price = float.Parse(collection["Price"]);
                product.Description = collection["Description"].ToString();

                long catID = long.Parse(collection["Category"]);

                Category cat = db.Categories.Where(c => c.Id == catID).FirstOrDefault();
                if (cat == null)
                {
                    return HttpNotFound();
                }
                product.Category = cat;

                //db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Hide(int id)
        {
            Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();

            if (product == null)
                return RedirectToAction("Index");

            product.status = "hidden";

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Activate(int id)
        {
            Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();
            
            if (product == null)
                return RedirectToAction("Index");

            product.status = "active";

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Where(c => c.Id == id).FirstOrDefault();

            if (product == null)
                return RedirectToAction("Index");
            else if (product.OrderProducts.Count != 0)
                return RedirectToAction("Index");

            foreach (CartProduct cp in product.CartProducts)
            {
                db.CartProducts.Remove(cp);
                db.SaveChanges();
            }

            foreach (Sale s in product.Sales)
            {
                db.Sales.Remove(s);
                db.SaveChanges();
            }

            foreach (Image i in product.Images)
            {
                db.Images.Remove(i);
                db.SaveChanges();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult Show(int id)
        {
            Product product = new Product();
            product = db.Products.Where(b => b.Id == id).FirstOrDefault();

            if (product == null)
                return RedirectToAction("Index", "Home");

            List<string> photos = new List<string>();
            Image photo = db.Images.Where(b => b.Product.Id == product.Id && b.Title == "Main").FirstOrDefault();
            if (photo != null)
            {
                photos.Add(photo.Source);
            }

            IList<Image> photos2 = db.Images.Where(b => b.Product.Id == product.Id && b.Title != "Main").ToList();
            foreach (Image p in photos2)
            {
                photos.Add(p.Source);
            }

            ViewBag.photoSrc = photos;
            ViewBag.categoryID = product.Category.Id;
            ViewBag.categoryName = product.Category.Name;

            foreach (Sale s in product.Sales)
            {
                if (s.startDate <= DateTime.Now && s.endDate >= DateTime.Now)
                {
                    ViewBag.sale = "true";
                    float temp = (float)(100 - s.Discount) / 100;
                    float tempsale = product.Price * temp;
                    float rounded = (float)(Math.Round((double)tempsale, 2));

                    ViewBag.salePrice = rounded.ToString("0.00");
                }
            }

            ViewBag.htmlDescription = product.Description;
            return View(product);
        }

        public ActionResult ShowList(int id)
        {
            ViewBag.currentPage = id;

            int pageSize = 9;
            IList<Product> products;

            string sale = Convert.ToString(Session["sale"]);
            float startP = (float)Convert.ToDouble(Session["startPrice"]);
            float endP = (float)Convert.ToDouble(Session["endPrice"]);

            int sortFlag = Convert.ToInt32(Session["sort"]);

            products = db.Products.ToList();

            if (sale == "true")
            {
                IList<Product> temp = new List<Product>();
                foreach (Product p in products)
                {
                    foreach (Sale s in p.Sales)
                    {
                        if (s.startDate <= DateTime.Now && s.endDate >= DateTime.Now)
                        {
                            temp.Add(p);
                            break;
                        }
                    }
                }
                products = temp;
            }

            if (startP > 0)
            {
                products = products.Where(c => c.Price >= startP).ToList();
            }

            if (endP > 0 && endP > startP)
            {
                products = products.Where(c => c.Price <= endP).ToList();

            }

            if (sortFlag == 1)               //od najpopulatniejszych
            {
                products = products.OrderByDescending(c => c.OrderProducts.Count).ToList();
                ViewBag.currentSort = "Popularność: od najpopularniejszych";
            }
            else if (sortFlag == 2)         //od najtanszych
            {
                products = products.OrderBy(c => c.Price).ToList();
                ViewBag.currentSort = "Cena: od najtańszych";
            }
            else if (sortFlag == 3)         //od najdrozszych
            {
                products = products.OrderByDescending(c => c.Price).ToList();
                ViewBag.currentSort = "Cena: od najdroższych";
            }
            else if (sortFlag == 4)         //po nazwie
            {
                products = products.OrderBy(c => c.Name).ToList();
                ViewBag.currentSort = "Nazwa: A-Z";
            }
            else                             //domyslnie
            {
                products = products.OrderByDescending(c => c.OrderProducts.Count).ToList();
                ViewBag.currentSort = "Popularność: od najpopularniejszych";
            }

            products = products.Where(p => p.status == "active").ToList();

            ViewBag.pageCount = products.Count / pageSize + 1;

            id = id - 1;
            int startRange = 0 + id * pageSize;
            int endRange = (pageSize - 1) + id * pageSize;

            IList<Product> pageProducts = new List<Product>();
            for (int i = 0; i < products.Count; i++)
            {
                if (i >= startRange && i <= endRange)
                    pageProducts.Add(products[i]);
                else if (i > endRange)
                    break;
            }

            ViewBag.pageP = pageProducts;

            ViewBag.pageSize = pageSize;

            return View(pageProducts);
        }

        public ActionResult ShowListByCategory(int categoryId, int id)
        {
            ViewBag.currentPage = id;

            int pageSize = 9;
            IList<Product> products;

            string sale = Convert.ToString(Session["sale"]);
            float startP = (float)Convert.ToDouble(Session["startPrice"]);
            float endP = (float)Convert.ToDouble(Session["endPrice"]);

            int sortFlag = Convert.ToInt32(Session["sort"]);

            int catId = categoryId;
            Category cat = db.Categories.Where(c => c.Id == catId).FirstOrDefault();
            if(cat == null)
            {
                return RedirectToAction("Index", "Home");
            }

            products = db.Products.Where(p => p.Category.Id == cat.Id).ToList();   

            products = products.Where(p => p.status == "active").ToList();
            

            ViewBag.categoryId = categoryId;
            ViewBag.categoryName = cat.Name;
            ViewBag.pageCount = products.Count / pageSize + 1;

            id = id - 1;
            int startRange = 0 + id * pageSize;
            int endRange = (pageSize - 1) + id * pageSize;

            IList<Product> pageProducts = new List<Product>();
            for (int i = 0; i < products.Count; i++)
            {
                if (i >= startRange && i <= endRange)
                    pageProducts.Add(products[i]);
                else if (i > endRange)
                    break;
            }

            ViewBag.pageP = pageProducts;

            ViewBag.pageSize = pageSize;

            return View(pageProducts);
        }


        public ActionResult ShowListByPromotions(int id)
        {
            ViewBag.currentPage = id;

            int pageSize = 9;
            IList<Product> products = new List<Product>(); ;

            string sale = Convert.ToString(Session["sale"]);
            float startP = (float)Convert.ToDouble(Session["startPrice"]);
            float endP = (float)Convert.ToDouble(Session["endPrice"]);

            int sortFlag = Convert.ToInt32(Session["sort"]);

            IList<Product> startProducts = db.Products.ToList();

            foreach (Product product in startProducts)
            {
                foreach (Sale s in product.Sales)
                {
                    if (s.startDate <= DateTime.Now && s.endDate >= DateTime.Now)
                    {
                        products.Add(product);
                    }
                }
            }
           
            products = products.Where(p => p.status == "active").ToList();

            ViewBag.pageCount = products.Count / pageSize + 1;

            id = id - 1;
            int startRange = 0 + id * pageSize;
            int endRange = (pageSize - 1) + id * pageSize;

            IList<Product> pageProducts = new List<Product>();
            for (int i = 0; i < products.Count; i++)
            {
                if (i >= startRange && i <= endRange)
                    pageProducts.Add(products[i]);
                else if (i > endRange)
                    break;
            }

            ViewBag.pageP = pageProducts;

            ViewBag.pageSize = pageSize;

            return View(pageProducts);
        }


        public PartialViewResult ShowFilters()
        {
            string sale = Convert.ToString(Session["sale"]);
            float startP = (float)Convert.ToDouble(Session["startPrice"]);
            float endP = (float)Convert.ToDouble(Session["endPrice"]);

            if (sale == "true")
                ViewBag.sale = "true";
            else
                ViewBag.sale = "false";

            if (startP <= 0)
                ViewBag.startP = "";
            else
                ViewBag.startP = startP;

            if (endP <= 0)
                ViewBag.endP = "";
            else
                ViewBag.endP = endP;

            return PartialView("~/Views/Product/ShowFilters.cshtml");
        }


        [HttpPost]
        public ActionResult ShowFilters(string onSale, float? startPrice, float? endPrice)
        {
            Session["endPrice"] = -1;
            Session["startPrice"] = -1;

            if (startPrice <= 0)
            {
                Session["startPrice"] = -1;
            }

            if (endPrice <= 0)
            {
                Session["endPrice"] = -1;
            }

            if (startPrice > 0)
            {
                Session["startPrice"] = startPrice;
            }

            if (endPrice > 0)
            {
                Session["endPrice"] = endPrice;
            }

            if (onSale == "on")
            {
                Session["sale"] = "true";
            }
            else
            {
                Session["sale"] = "false";
            }

            if (endPrice < startPrice)
            {
                Session["startPrice"] = -1;
                Session["endPrice"] = -1;
            }

            return RedirectToAction("ShowList", "Product", new { id = 1 });
        }

        public ActionResult SetSort(int id)
        {
            Session["sort"] = id;

            return RedirectToAction("ShowList", "Product", new { id = 1 });
        }

        public ActionResult ResetFilters()
        {
            Session["sale"] = "false";
            Session["startPrice"] = -1;
            Session["endPrice"] = -1;

            return RedirectToAction("ShowList", "Product", new { id = 1 });
        }

        public ActionResult SearchResult(int id, string name)
        {
            ViewBag.currentPage = id;

            int pageSize = 9;
            IList<Product> products;
            products = db.Products.Where(p => p.Name.Contains(name)).ToList();

            ViewBag.searchName = name;
            ViewBag.pageCount = products.Count / pageSize + 1;

            id = id - 1;
            int startRange = 0 + id * pageSize;
            int endRange = (pageSize - 1) + id * pageSize;

            IList<Product> pageProducts = new List<Product>();
            for (int i = 0; i < products.Count; i++)
            {
                if (i >= startRange && i <= endRange)
                    pageProducts.Add(products[i]);
                else if (i > endRange)
                    break;
            }

            ViewBag.pageP = pageProducts;

            ViewBag.pageSize = pageSize;

            return View(pageProducts);
        }

        [HttpPost]
        public ActionResult Search(string name)
        {
            return RedirectToAction("SearchResult", "Product", new { id = 1, name = name });
        }

        public PartialViewResult ProductCard(int id)
        {
            Product product;
            product = db.Products.Where(b => b.Id == id).FirstOrDefault();
            if(product == null)
            {
                return PartialView("~/Views/Product/ProductCard.cshtml", product);
            }

            Image photo = db.Images.Where(b => b.Product.Id == product.Id && b.Title == "Main").FirstOrDefault();
            if (photo == null)
            {
                Image photo2 = db.Images.Where(b => b.Product.Id == product.Id).FirstOrDefault();
                if(photo2 == null)
                    ViewBag.photoSrc = null;
                else
                    ViewBag.photoSrc = photo2.Source;
            }
            else
            {
                ViewBag.photoSrc = photo.Source;
            }

            foreach (Sale s in product.Sales)
            {
                if (s.startDate <= DateTime.Now && s.endDate >= DateTime.Now)
                {
                    ViewBag.sale = "true";
                    float temp = (float)(100 - s.Discount) / 100;
                    float tempsale = product.Price * temp;
                    float rounded = (float)(Math.Round((double)tempsale, 2));

                    ViewBag.salePrice = rounded.ToString("0.00");

                    return PartialView("~/Views/Product/ProductCard.cshtml", product);
                }
            }

            return PartialView("~/Views/Product/ProductCard.cshtml", product);
        }

        public bool validateProduct(string productName, string productPrice, string productDescription)
        {
            bool validate = true;

            if (productName == null)
            {
                ViewBag.productNameError = "Pole nie może być puste";
                validate = false;
            }
            else if (productName.Trim().Length == 0)
            {
                ViewBag.productNameError = "Pole nie może być puste";
                validate = false;
            }
            else if (productName.Length >= 70)
            {
                ViewBag.productNameError = "Nazwa jest za długa";
                validate = false;
            }


            if (productDescription == null)
            {
                ViewBag.productDesError = "Pole nie może być puste";
                validate = false;
            }
            else if (productDescription.Trim().Length == 0)
            {
                ViewBag.productDesError = "Pole nie może być puste";
                validate = false;
            }
            else if (productDescription.Length >= 700)
            {
                ViewBag.productDesError = "Opis jest za długi";
                validate = false;
            }

            string number = productPrice.ToString();
            int length = 0;

            if (number.Contains(","))
                length = number.Substring(number.IndexOf(",") + 1).Length;

            if (length > 2 || productPrice.Trim().Length == 0 || number.Contains("."))
            {
                ViewBag.productPriceError = "Niedozwolona cena";
                validate = false;
            }
            else if (float.Parse(productPrice, CultureInfo.InvariantCulture.NumberFormat) <= 0)
            {
                ViewBag.productPriceError = "Niedozwolona cena";
                validate = false;
            }

            return validate;
        }
    }


}