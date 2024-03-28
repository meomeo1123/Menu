using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;
using PagedList.Mvc;


namespace tester1.Controllers
{
    public class UserController : Controller
    {
        ThucDonDataContext db = new ThucDonDataContext();

        public ActionResult Index(int? page)
        {
            var userId = User.Identity.GetUserId();
            var items = db.DonHangs.Where(dh => dh.IdUser == userId).OrderByDescending(dh => dh.NgayDatHang);

            int pageNumber = page ?? 1;
            int pageSize = 10; // Số mục trên mỗi trang

            // Áp dụng phân trang
            IPagedList<DonHang> pagedItems = items.ToPagedList(pageNumber, pageSize);

            return View(pagedItems);
        }
        public ActionResult View(string id)
        {
            var item = db.DonHangs.FirstOrDefault(x => x.MaDH == id);
            return View(item);
        }
        public ActionResult Partial_SanPham(string id)
        {
            var items = db.ChiTietDonHangs.Where(x => x.MaDH == id).ToList();
            var orderStatus = db.DonHangs.FirstOrDefault(x => x.MaDH == id)?.TrangThaiDonHang ?? 0; // Giả sử TrangThai là trường lưu trạng thái đơn hàng
            ViewBag.OrderStatus = orderStatus; // Truyền trạng thái đơn hàng tới view
            return PartialView(items);
        }
        public ActionResult HuyDonHang(string orderId)
        {
            // Kiểm tra trạng thái đơn hàng có phù hợp để huỷ hay không
            var order = db.DonHangs.FirstOrDefault(o => o.MaDH == orderId);
            if (order.TrangThaiDonHang == 1 || order.TrangThaiDonHang == 2)
            {
                // Lấy danh sách chi tiết đơn hàng có liên quan
                order.TrangThaiDonHang = 0;
                order.NgayHuy = DateTime.Now;

                // Lưu thay đổi
                db.SubmitChanges();

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Huỷ đơn hàng thành công" }, JsonRequestBehavior.AllowGet);
            }

            // Trả về kết quả thất bại
            return Json(new { success = false, message = "Không thể huỷ đơn hàng" }, JsonRequestBehavior.AllowGet);
        }

    }
}