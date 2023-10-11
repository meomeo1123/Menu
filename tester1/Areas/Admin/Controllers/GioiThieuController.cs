    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    public class GioiThieuController : Controller
    {
        // GET: Admin/GioiThieu
        ThucDonDataContext db = new ThucDonDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult GioiThieu()
        {
            var gioiThieu = db.GioiThieus.OrderByDescending(x => x.NgayDang).ToList();
            return View(gioiThieu);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection collection, GioiThieu s)
        {
            var e_gioithieu = Convert.ToInt32(collection["IdGT"]);
            var e_tieude = collection["TieuDe"];
            var e_view = 1;
            var e_noidung = collection["NoiDung"];
            var e_ngaydang = Convert.ToDateTime(collection["NgayDang"]);
            if (string.IsNullOrEmpty(e_tieude))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.IdGT = e_gioithieu;
                s.TieuDe = e_tieude.ToString();
                s.NoiDung = e_noidung.ToString();
                s.View = e_view;
                s.NgayDang = DateTime.Now;
                db.GioiThieus.InsertOnSubmit(s);
                db.SubmitChanges();
                return RedirectToAction("GioiThieu");
            }
            return this.Create();
        }

        public ActionResult Edit(int id)
        {
            var gioiThieu = db.GioiThieus.FirstOrDefault(m => m.IdGT == id);

            if (gioiThieu == null)
            {
                return HttpNotFound();
            }

            return View(gioiThieu);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_gioithieu = db.GioiThieus.FirstOrDefault(m => m.IdGT == id);

            if (E_gioithieu == null)
            {
                return HttpNotFound();
            }
            var E_Tieude = collection["TieuDe"];
            var E_NoiDung = collection["NoiDung"];
            var e_ngaydang = Convert.ToDateTime(collection["NgayDang"]);

            if (string.IsNullOrEmpty(E_Tieude))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_gioithieu.NgayDang = e_ngaydang;
                E_gioithieu.TieuDe = E_Tieude;
                E_gioithieu.NoiDung = E_NoiDung;
                db.SubmitChanges();
                return RedirectToAction("GioiThieu");
            }

            return View(E_gioithieu);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var E_gioithieu = db.GioiThieus.SingleOrDefault(m => m.IdGT == id);
            return View(E_gioithieu);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            // Tạo SelectList cho danh sách danh mục
            var gioiThieu = db.GioiThieus.SingleOrDefault(m => m.IdGT == id);
            db.GioiThieus.DeleteOnSubmit(gioiThieu);
            db.SubmitChanges();
            return RedirectToAction("GioiThieu");
        }

    }
}