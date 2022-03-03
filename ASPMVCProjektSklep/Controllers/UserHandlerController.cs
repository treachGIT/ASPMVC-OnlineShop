using ASPMVCProjektSklep.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVCProjektSklep.Controllers
{
    public class UserHandlerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var user = db.Users.ToList();


            string[] userNames = new string[user.Count];
            string[] userRoles = new string[user.Count];
            int i = 0;
            foreach (var u in user)
            {
                userNames[i] = u.Email;

                var alluserRoles = u.Roles.ToList();

                bool admin = false;
                foreach (var role in alluserRoles)
                {
                    var roleId = role.RoleId;
                    var concreteRole = db.Roles.Where(r => r.Id == roleId).FirstOrDefault();
                    if(concreteRole.Name == "Admin")
                    {
                        admin = true;
                    }
                }

                if (admin == true)
                    userRoles[i] = "Admin";
                else
                    userRoles[i] = "User";
                    
                i++;
            }

            ViewBag.len = user.Count;
            ViewBag.userNames = userNames;
            ViewBag.userRoles = userRoles;

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult MakeAdmin(string name)
        {
            IdentityManager im = new IdentityManager();

            var user = db.Users.Where(u => u.Email == name).FirstOrDefault();
            var userid = user.Id;
                
            im.AddUserToRoleByUsername(name, "Admin");

            return RedirectToAction("Index");
        }

        public ActionResult Admin(string name)
        {
            IdentityManager im = new IdentityManager();

            var user = db.Users.Where(u => u.Email == name).FirstOrDefault();
            var userid = user.Id;

            im.AddUserToRoleByUsername(name, "Admin");

            return RedirectToAction("Index");
        }

    }
}