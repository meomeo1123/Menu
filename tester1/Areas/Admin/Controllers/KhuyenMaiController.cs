using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    public class KhuyenMaiController : Controller
    {
        // GET: Admin/KhuyenMai
        ThucDonDataContext db = new ThucDonDataContext();
        public ActionResult KhuyenMai()
        {
            var all_KM = db.KhuyenMais.Where(dm => dm.IdKm >= 0).ToList();
            return View(all_KM);
        }
        // create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(FormCollection collection, KhuyenMai s)
        {
            var E_IdKM = Convert.ToInt32(collection["IdKM"]);
            var E_NameKm = collection["NameKm"];
            var E_StarTime = Convert.ToDateTime(collection["StartTime"]);
            var E_EndTime = Convert.ToDateTime(collection["EndTime"]);
            var E_Promo = Convert.ToInt32(collection["PromoSale"]);

            if (string.IsNullOrEmpty(E_NameKm))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else if (E_StarTime >= E_EndTime)
            {
                ViewData["Error"] = "Start time must be earlier than end time!";
            }
            else
            {
                s.IdKm = E_IdKM;
                s.NameKm = E_NameKm;
                s.StartTime = E_StarTime; // Gán giá trị kiểu DateTime
                s.EndTime = E_EndTime; // Gán giá trị kiểu DateTime
                s.PromoSale = E_Promo;
                db.KhuyenMais.InsertOnSubmit(s);
                db.SubmitChanges();
                return RedirectToAction("KhuyenMai");
            }

            // Hiển thị trang tạo mới với thông báo lỗi (nếu có)
            return View(s);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var E_khuyenmai = db.KhuyenMais.First(m => m.IdKm == id);
            return View(E_khuyenmai);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_khuyenmai = db.KhuyenMais.First(m => m.IdKm == id);
            var E_IdKm = Convert.ToInt32(collection["IdKM"]);
            var E_TenKm = collection["NameKm"];
            E_khuyenmai.IdKm = id;
            if (string.IsNullOrEmpty(E_TenKm))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_khuyenmai.NameKm = E_TenKm;
                UpdateModel(E_khuyenmai);
                db.SubmitChanges();
                return RedirectToAction("KhuyenMai");
            }
            return this.Edit(id);
        }

        //---------------detele-----------------
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var D_khuyenmai = db.KhuyenMais.First(m => m.IdKm == id);
            return View(D_khuyenmai);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_khuyenmai = db.KhuyenMais.Where(m => m.IdKm == id).First();
            db.KhuyenMais.DeleteOnSubmit(D_khuyenmai);
            db.SubmitChanges();
            return RedirectToAction("KhuyenMai");
        }
    }
}