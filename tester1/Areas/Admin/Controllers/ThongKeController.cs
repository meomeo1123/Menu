using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ThongKeController : Controller
    {
        ThucDonDataContext db = new ThucDonDataContext();

        // thống kê đơn hàng 
        public JsonResult GetOrdersData(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Nếu không có ngày nào được chọn, sử dụng ngày mặc định là 4 ngày trước ngày hiện tại
                endDate = DateTime.Today;
                startDate = endDate.Value.AddDays(-3);
            }
            var items = db.DonHangs.Where(dh => dh.TrangThaiDonHang == 5 &&
                                                dh.NgayDatHang >= startDate &&
                                                dh.NgayDatHang <= endDate)
                                    .OrderByDescending(dh => dh.NgayDatHang)
                                    .ToList();

            var dailyOrders = items.GroupBy(dh => dh.NgayDatHang.Value.Date)
                                    .Select(group => new { Date = group.Key.ToString("dd/MM/yyyy"), Count = group.Count() })
                                    .OrderBy(entry => entry.Date)
                                    .ToList();

            return Json(dailyOrders, JsonRequestBehavior.AllowGet);
        }

        // hiển thị dữ liệu api

        public ActionResult ShowDataGetOrder()
        {
            return View();
        }

        // thống kê doanh thu

        public ActionResult RevenueStatistics(DateTime? startDate, DateTime? endDate)
        {
            // Nếu không có giá trị fromDate và toDate, sử dụng giá trị mặc định là 4 ngày gần đây
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Nếu không có ngày nào được chọn, sử dụng ngày mặc định là 4 ngày trước ngày hiện tại
                endDate = DateTime.Today;
                startDate = endDate.Value.AddDays(-3);
            }

            var items = db.DonHangs.Where(dh => dh.TrangThaiDonHang == 5 &&
                                                dh.NgayDatHang >= startDate &&
                                                dh.NgayDatHang <= endDate)
                                    .OrderByDescending(dh => dh.NgayDatHang)
                                    .ToList();

            var dailyRevenue = items.GroupBy(dh => dh.NgayDatHang.Value.Date)
                             .Select(group => new
                             {
                                 Date = group.Key,
                                 Revenue = group.Sum(dh => dh.TongTien),
                                 Cost = group.Sum(dh => dh.ChiTietDonHangs.Sum(ct => ct.SanPham.GiaNhap * ct.SoLuong))
                             })
                             .OrderBy(entry => entry.Date)
                             .ToList();
            return View(items);
        }

        public ActionResult FilteredRevenueStatistics2(DateTime fromDate, DateTime toDate)
        {
            var items = db.DonHangs.Where(dh => dh.TrangThaiDonHang == 5 &&
                                                dh.NgayDatHang >= fromDate &&
                                                dh.NgayDatHang <= toDate)
                                    .OrderByDescending(dh => dh.NgayDatHang)
                                    .ToList();

            var dailyRevenue = items.GroupBy(dh => dh.NgayDatHang.Value.Date)
                                     .Select(group => new
                                     {
                                         Date = group.Key.ToString("dd/MM/yyyy"),
                                         Revenue = group.Sum(dh => dh.TongTien),
                                         Cost = group.Sum(dh => dh.ChiTietDonHangs.Sum(ct => ct.SanPham.GiaNhap * ct.SoLuong))
                                     })
                                     .OrderBy(entry => entry.Date)
                                     .ToList();

            ViewBag.Dates = dailyRevenue.Select(entry => entry.Date);
            ViewBag.Revenues = dailyRevenue.Select(entry => entry.Revenue);
            ViewBag.Costs = dailyRevenue.Select(entry => entry.Cost);
            ViewBag.Profits = dailyRevenue.Select(entry => entry.Revenue - entry.Cost);

            return View();
        }

        public PartialViewResult CategoryP(int? categoryId)
        {
            ViewBag.Categories = db.DanhMucs.ToList();

            if (categoryId.HasValue)
            {
                if (categoryId.Value != 0)
                {
                    var products = db.SanPhams.Include(p => p.DanhMuc)
                          .Where(p => p.MaDM == categoryId.Value)
                          .ToList();
                    return PartialView(products);
                }
                else
                {
                    var products = (from p in db.SanPhams
                                    join s in db.TrangThais on p.IdS equals s.IdS
                                    join c in db.DanhMucs on p.MaDM equals c.MaDM
                                    select new Product
                                    {
                                        NoiDung = p.NoiDung,
                                        MaDM = c.MaDM,
                                        MaSP = p.MaSP,
                                        TenSP = p.TenSP,
                                        GiaBan = ((float)p.GiaBan.Value),
                                        MoTa = p.MoTa,
                                        Hinh = p.HinhAnh,
                                        TenDM = c.TenDM,
                                        Status = s.Status
                                    }
                                ).ToList();

                    return PartialView(products);
                }
            }
            else
            {
                var products = (from p in db.SanPhams
                                join s in db.TrangThais on p.IdS equals s.IdS
                                join c in db.DanhMucs on p.MaDM equals c.MaDM
                                select new Product
                                {
                                    MaDM = c.MaDM,
                                    MaSP = p.MaSP,
                                    TenSP = p.TenSP,
                                    GiaBan = ((float)p.GiaBan.Value),
                                    GiaNhap = ((float)p.GiaNhap.Value),
                                    MoTa = p.MoTa,
                                    Hinh = p.HinhAnh,
                                    TenDM = c.TenDM,
                                    Status = s.Status
                                }
                            ).ToList();

                return PartialView(products);
            }
        }


        public JsonResult LayDanhSachDonHang(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Nếu không có ngày nào được chọn, sử dụng ngày mặc định là 4 ngày trước ngày hiện tại
                endDate = DateTime.Today;
                startDate = endDate.Value.AddDays(-4);
            }

            var donHangs = db.DonHangs
                .Where(dh => dh.TrangThaiDonHang == 5 && dh.NgayDatHang >= startDate && dh.NgayDatHang <= endDate) // Lọc đơn hàng theo ngày
                .Select(dh => new
                {
                    dh.MaDH,
                    dh.TongTien,
                    ChiTietDonHangs = dh.ChiTietDonHangs
                        .Select(ct => new
                        {
                            ct.MaDH,
                            ct.MaSP,
                            ct.TenSP,
                        })
                })
                .ToList();
            var tongSoLuongDonHang = donHangs.Count; // Tính tổng số đơn hàng sau khi đã lọc
            return Json(new { TongSoLuongDonHang = tongSoLuongDonHang, DonHangs = donHangs }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ThongKeSp(int id, DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Nếu không có ngày nào được chọn, sử dụng ngày mặc định là 4 ngày trước ngày hiện tại
                endDate = DateTime.Today;
                startDate = endDate.Value.AddDays(-4);
            }
            Session["IdSp"] = id;
            // Kiểm tra và xử lý các giá trị startDate và endDate
            // ... (code xử lý ngày tháng)

            var donHangsAll = db.DonHangs
                .Where(dh => dh.TrangThaiDonHang == 5 && dh.NgayDatHang >= startDate && dh.NgayDatHang <= endDate)
                .Select(dh => new
                {
                    dh.MaDH,
                    dh.TongTien,
                    ChiTietDonHangs = dh.ChiTietDonHangs
                        .Select(ct => new
                        {
                            ct.MaDH,
                            ct.MaSP,
                            ct.TenSP,
                        })
                })
                .ToList();

            var donHangsFiltered = donHangsAll
                .Where(dh => dh.ChiTietDonHangs.Any(ctdh => ctdh.MaSP == Convert.ToInt32(Session["IdSp"])))
                .Select(dh => new
                {
                    dh.MaDH,
                    dh.TongTien,
                })
                .ToList();

            var tenSanPham = db.ChiTietDonHangs
            .Where(ct => ct.MaSP == id)
            .Select(ct => ct.TenSP)
            .FirstOrDefault();

            if (tenSanPham != null)
            {
                Session["TenSanPham"] = tenSanPham;
            }
            var tongSoLuongDonHang = donHangsAll.Count;
            var tongSoDonHangId = donHangsFiltered.Count;
            Session["TongSoLuongDonHang"] = tongSoLuongDonHang;
            Session["TongSoDonHangId"] = tongSoDonHangId;
            return Json(new { TongSoLuongDonHang = tongSoLuongDonHang, TongSoDonHangId = tongSoDonHangId }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SpTheoId()
        {
            var tongSoLuongDonHang = Session["TongSoLuongDonHang"];
            var tongSoDonHangId = Session["TongSoDonHangId"];
            // Truyền dữ liệu xuống view
            ViewBag.TongSoLuongDonHang = tongSoLuongDonHang;
            ViewBag.TongSoDonHangId = tongSoDonHangId;

            return View();
        }

        // lượng truy cập

        public JsonResult ViewCout(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                // Nếu không có ngày nào được chọn, sử dụng ngày mặc định
                endDate = DateTime.Today;
                startDate = endDate.Value.AddMonths(-1);
            }

            var accessData = db.AccessCounts
                .Where(a => a.DateTime >= startDate && a.DateTime <= endDate)
                .Select(a => new
                {
                    a.DateTime,
                    a.TotalAcess
                })
                .ToList();

            return Json(accessData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewAccess()
        {
            return View();
        }

    }
}
