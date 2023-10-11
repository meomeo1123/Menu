using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    public class DanhMucController : Controller
    {
        // GET: DanhMuc
         ThucDonDataContext data = new ThucDonDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult DanhMuc()
        {
            var all_danhmuc = data.DanhMucs.Where(dm => dm.MaDM >= 0).ToList();
            return View(all_danhmuc);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(FormCollection collection, DanhMuc s)
        {
            var E_MaDanhMuc = Convert.ToInt32(collection["MaDM"]);
            var E_TenDanhMuc = collection["TenDM"];
            if (string.IsNullOrEmpty(E_TenDanhMuc))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.MaDM = E_MaDanhMuc;
                s.TenDM = E_TenDanhMuc.ToString();
                data.DanhMucs.InsertOnSubmit(s);
                data.SubmitChanges();
                return RedirectToAction("DanhMuc");
            }
            return this.Create();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var E_danhmuc = data.DanhMucs.First(m => m.MaDM == id);
            return View(E_danhmuc);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_danhmuc = data.DanhMucs.First(m => m.MaDM == id);
            var E_MaDanhMuc = Convert.ToInt32(collection["MaDM"]);
            var E_TenDanhMuc = collection["TenDM"];
            E_danhmuc.MaDM = id;
            if (string.IsNullOrEmpty(E_TenDanhMuc))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_danhmuc.TenDM = E_TenDanhMuc;
                UpdateModel(E_danhmuc);
                data.SubmitChanges();
                return RedirectToAction("DanhMuc");
            }
            return this.Edit(id);
        }

        //---------------detele-----------------
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var D_danhmuc = data.DanhMucs.First(m => m.MaDM == id);
            return View(D_danhmuc);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_danhmuc = data.DanhMucs.Where(m => m.MaDM == id).First();
            data.DanhMucs.DeleteOnSubmit(D_danhmuc);
            data.SubmitChanges();
            return RedirectToAction("DanhMuc");
        }
    }

}

