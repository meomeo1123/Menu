using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {
        // GET: Admin/DonHang
        ThucDonDataContext db = new ThucDonDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page, int? trangThai)
        {
            var items = db.DonHangs.OrderByDescending(x => x.NgayDatHang).ToList();

            if (trangThai.HasValue)
            {
                items = items.Where(dh => dh.TrangThaiDonHang == trangThai.Value).ToList();
            }

            var pageNumber = page ?? 1;
            var pageSize = 10;
            var pagedItems = items.ToPagedList(pageNumber, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;

            return View(pagedItems);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult View(string id)
        {
            var item = db.DonHangs.FirstOrDefault(x => x.MaDH == id);
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Partial_SanPham(string id)
        {
            var items = db.ChiTietDonHangs.Where(x => x.MaDH == id).ToList();
            return PartialView(items);
        }

        // quy ước 1 : đơn hàng mới , 2: đã xác nhận , 3: đang chuẩn bị món ăn , 4: đang vận chuyển đến bạn , 5 : thành công , 0 : huỷ đơn 
        // đơn hàng ở trạng thái 1 2 mới được huỷ nhe 
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateStatus(string orderId, int status)
        {
            var order = db.DonHangs.FirstOrDefault(o => o.MaDH == orderId);
            if (order != null)
            {
                // Cập nhật trạng thái đơn hàng
                order.TrangThaiDonHang = status;

                // Lưu thay đổi vào CSDL
                db.SubmitChanges();

                // Trả về kết quả thành công
                return Json(new { success = true });
            }
            // Trả về kết quả thất bại nếu không tìm thấy đơn hàng
            return Json(new { success = false });
        }

        [Authorize(Roles = "Admin")]
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