using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    public class ListUserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        public ListUserController()
        {
        }

        public ListUserController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ThucDonDataContext db = new ThucDonDataContext();
            var listNd = db.AspNetUsers.OrderBy(s => s.Id).ToList();
            return View(listNd);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            using (ThucDonDataContext db = new ThucDonDataContext())
            {
                var user = db.AspNetUsers.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var userList = new List<AspNetUser> { user };
                return View(userList);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            ThucDonDataContext db = new ThucDonDataContext();
            var E_category = db.AspNetUsers.SingleOrDefault(m => m.Id == id);
            return View(E_category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id, FormCollection collection)
        {
            ThucDonDataContext db = new ThucDonDataContext();
            var useredit = db.AspNetUsers.SingleOrDefault(m => m.Id == id);
            var E_user = collection["UserName"];
            useredit.Id = id;
            if (string.IsNullOrEmpty(E_user))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                useredit.UserName = E_user;
                UpdateModel(useredit);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Edit(id);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            ThucDonDataContext db = new ThucDonDataContext();
            var user = db.AspNetUsers.SingleOrDefault(m => m.Id == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(string id)
        {
            ThucDonDataContext db = new ThucDonDataContext();
            var user = db.AspNetUsers.SingleOrDefault(m => m.Id == id);

            if (user == null)
            {
                return HttpNotFound();
            }

            db.AspNetUsers.DeleteOnSubmit(user);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }
    }
}