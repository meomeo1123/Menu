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
            var khuyenMaiWithStatusList = (
                  from km in db.KhuyenMais
                  join ql in db.QuanLiKMs on km.IdKm equals ql.IdKm
                  select new QLKMai
                  {
                      KhuyenMai = km,
                      Status = ql.Status.ToString() // Chuyển trạng thái sang kiểu string
                }
                ).ToList();
            return View (khuyenMaiWithStatusList);
        }

        // create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var quanli = db.QuanLiKMs.Where(id => id.Idlink >= 0).ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(FormCollection collection, KhuyenMai s , QuanLiKM q)
        {
            var E_IdKM = Convert.ToInt32(collection["IdKM"]);
            var E_NameKm = collection["NameKm"];
            var E_StarTime = Convert.ToDateTime(collection["StartTime"]);
            var E_EndTime = Convert.ToDateTime(collection["EndTime"]);
            var E_Promo = Convert.ToInt32(collection["PromoSale"]);
            var E_Quantity = Convert.ToInt32(collection["Quantity"]);



            var E_Idlink = Convert.ToInt32(collection["Idlink"]);
            var E_IdK = Convert.ToInt32(collection["IdKM"]);
             var   E_Status = collection["Status"];
      

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
                s.Quantity = E_Quantity;
                db.KhuyenMais.InsertOnSubmit(s);
                db.SubmitChanges();


                q.Idlink = E_Idlink;
                q.IdKm = s.IdKm;   // Gán ID khuyến mãi cho chi tiết khuyến mãi
                q.Status = E_Status; // Đặt trạng thái mặc định
                db.QuanLiKMs.InsertOnSubmit(q);
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
            var E_Chitiet = db.QuanLiKMs.FirstOrDefault(chitiet => chitiet.IdKm == E_khuyenmai.IdKm);
            return View(E_khuyenmai);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_khuyenmai = db.KhuyenMais.First(m => m.IdKm == id);
            var E_Chitiet = db.QuanLiKMs.FirstOrDefault(chitiet => chitiet.IdKm == E_khuyenmai.IdKm);
            var E_IdKm = Convert.ToInt32(collection["IdKM"]);
            var E_TenKm = collection["NameKm"];
            var E_Status = collection["Status"];
            var E_Quantity = collection["Quantity"];
            E_khuyenmai.IdKm = id;

            if (string.IsNullOrEmpty(E_TenKm))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_khuyenmai.NameKm = E_TenKm;
                E_Chitiet.Status = E_Status;
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
            var E_Chitiet = db.QuanLiKMs.FirstOrDefault(chitiet => chitiet.IdKm == D_khuyenmai.IdKm);
            return View(D_khuyenmai);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
                var D_khuyenmai = db.KhuyenMais.FirstOrDefault(m => m.IdKm == id);
                if (D_khuyenmai != null)
                {
                    var E_Chitiet = db.QuanLiKMs.FirstOrDefault(chitiet => chitiet.IdKm == D_khuyenmai.IdKm);

                    if (E_Chitiet != null)
                    {
                        db.KhuyenMais.DeleteOnSubmit(D_khuyenmai);
                        db.QuanLiKMs.DeleteOnSubmit(E_Chitiet);

                        db.SubmitChanges();
                    }
                }
                return RedirectToAction("KhuyenMai");
            
        }
    }
}