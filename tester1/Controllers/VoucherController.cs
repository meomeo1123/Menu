using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using tester1.Models;
using System.Data.Entity;

namespace tester1.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Voucher
        ThucDonDataContext db = new ThucDonDataContext();
        private readonly UserManager<ApplicationUser> _userManager;

        private bool CheckIfPromotionIsSaved(string userId, int promotionId)
        {
            // Truy vấn CSDL để kiểm tra xem mã đã được lưu hay chưa
            var chiTietSale = db.ChiTietSales.FirstOrDefault(c => c.idUser == userId && c.IdKm == promotionId);

            // Nếu chi tiết sale không null, tức là mã đã được lưu
            return chiTietSale != null;
        }
        public ActionResult Voucher()
        {
            var userId = User.Identity.GetUserId();
            var currentTime = DateTime.Now; // Lấy thời gian hiện tại

            var vouchers = (
                from km in db.KhuyenMais
                join ql in db.QuanLiKMs on km.IdKm equals ql.IdKm
                where km.EndTime > currentTime && km.Quantity > 0 // Kiểm tra điều kiện EndTime > thời gian hiện tại và quantity > 0
                select new QLKMai
                {
                    KhuyenMai = km,
                    Status = ql.Status.ToString(),
                    IsSaved = CheckIfPromotionIsSaved(userId, km.IdKm) // Sử dụng phương thức để kiểm tra
                 }).ToList();

            return View(vouchers);
        }


        [HttpGet]
        public ActionResult SavePromotion()
        {
             
            return View();
        }

        [HttpPost]
        public ActionResult SavePromotion(int promotionId)
        {
            var userId = User.Identity.GetUserId();
            if (userId != null)
            {
                var chiTietSale = new ChiTietSale();
                chiTietSale.idUser = userId;
                chiTietSale.IdKm = promotionId;
                chiTietSale.Status = 1;
                // 1 là chưa sử dụng 
                // 0 là đã sử dụng 

                db.ChiTietSales.InsertOnSubmit(chiTietSale);
                db.SubmitChanges();
                var promotion = db.KhuyenMais.FirstOrDefault(km => km.IdKm == promotionId);
                if (promotion != null && promotion.Quantity > 0)
                {
                    // Giảm quantity đi 1 nếu quantity hiện tại lớn hơn 0
                    promotion.Quantity -= 1;

                    // Cập nhật lại thông tin trong cơ sở dữ liệu
                    db.SubmitChanges();

                    // Thông điệp thành công
                    ViewBag.SuccessMessage = "Lưu mã thành công!";
                }

                // Thông điệp thành công
                ViewBag.SuccessMessage = "Lưu mã thành công!";
            }
            else
            {
                // Xử lý khi userId là null
                ViewBag.ErrorMessage = "Không thể xác định được userId.";
            }

            // Load lại trang hoặc chuyển hướng đến trang khác nếu cần
            var all_vch = (
                from km in db.KhuyenMais
                join ql in db.QuanLiKMs on km.IdKm equals ql.IdKm
                select new QLKMai
                {
                    KhuyenMai = km,
                    Status = ql.Status.ToString() // Chuyển trạng thái sang kiểu string
                }
            ).ToList();

            return View("Voucher", all_vch);
        }



    }


}