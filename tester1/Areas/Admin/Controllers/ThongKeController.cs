using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    public class ThongKeController : Controller
    {
        ThucDonDataContext db = new ThucDonDataContext();

        // GET: Admin/ThongKe
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            DateTime fromDate = DateTime.Now.AddDays(-3);
            DateTime toDate = DateTime.Now;

            var items = db.DonHangs.Where(dh => dh.TrangThaiDonHang == 5 &&
                                                dh.NgayDatHang >= fromDate &&
                                                dh.NgayDatHang <= toDate)
                                    .OrderByDescending(dh => dh.NgayDatHang)
                                    .ToList();

            var dailyOrders = items.GroupBy(dh => dh.NgayDatHang.Value.Date)
                                    .Select(group => new { Date = group.Key, Count = group.Count() })
                                    .OrderBy(entry => entry.Date)
                                    .ToList();

            ViewBag.Dates = dailyOrders.Select(entry => entry.Date.ToString("dd/MM/yyyy"));
            ViewBag.Quantities = dailyOrders.Select(entry => entry.Count);

            return View(items);
        }

        public ActionResult FilteredOrders(DateTime fromDate, DateTime toDate)
        {
            var items = db.DonHangs.Where(dh => dh.TrangThaiDonHang == 5 &&
                                                dh.NgayDatHang >= fromDate &&
                                                dh.NgayDatHang <= toDate)
                                    .OrderByDescending(dh => dh.NgayDatHang)
                                    .ToList();

            var dailyOrders = items.GroupBy(dh => dh.NgayDatHang.Value.Date)
                                    .Select(group => new { Date = group.Key, Count = group.Count() })
                                    .OrderBy(entry => entry.Date)
                                    .ToList();

            ViewBag.Dates = dailyOrders.Select(entry => entry.Date.ToString("dd/MM/yyyy"));
            ViewBag.Quantities = dailyOrders.Select(entry => entry.Count);

            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult RevenueStatistics(DateTime? fromDate, DateTime? toDate)
        {
            // Nếu không có giá trị fromDate và toDate, sử dụng giá trị mặc định là 4 ngày gần đây
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.AddDays(-4);
                toDate = DateTime.Now;
            }

            var items = db.DonHangs.Where(dh => dh.TrangThaiDonHang == 5 &&
                                                dh.NgayDatHang >= fromDate &&
                                                dh.NgayDatHang <= toDate)
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

            ViewBag.Dates = dailyRevenue.Select(entry => entry.Date.ToString("dd/MM/yyyy"));
            ViewBag.Revenues = dailyRevenue.Select(entry => entry.Revenue);
            ViewBag.Costs = dailyRevenue.Select(entry => entry.Cost);
            ViewBag.Profits = dailyRevenue.Select(entry => entry.Revenue - entry.Cost);

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(items);
        }

        [Authorize(Roles = "Admin")]
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

    }
}