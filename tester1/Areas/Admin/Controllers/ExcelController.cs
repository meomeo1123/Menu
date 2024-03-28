using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Web.Mvc;
using tester1.Models;
using System.IO;
using System.Diagnostics;

namespace tester1.Areas.Admin.Controllers
{
    public class ExcelController : Controller
    {
        ThucDonDataContext db = new ThucDonDataContext();
        // GET: Admin/Excel
        public string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public ActionResult getIdOrder(List<string> ids)
        {
            var orders = db.DonHangs.Where(o => ids.Contains(o.MaDH)).ToList();
            var orderDetails = new List<Excel.OrderDetailViewModel>();

            foreach (var order in orders)
            {
                var orderDetail = new Excel.OrderDetailViewModel
                {
                    MaDH = order.MaDH,
                    HoTen = order.Ten,
                    DiaChi = order.DiaChi,
                    SoDT = order.Sdt,
                    TongTien = Convert.ToInt32(order.TongTien),
                    TrangThai = Convert.ToInt32(order.TrangThaiDonHang),
                    GhiChu = order.Ghichu,
                    NgayDat = FormatDateTime(Convert.ToDateTime(order.NgayDatHang)),
                    Email = order.Email,
                    NgayHuy = order.NgayHuy,
                    PPTT = order.PhuongThucThanhToan,
                };
                orderDetail.ChiTietDonHangs = db.ChiTietDonHangs
                 .Where(chiTiet => chiTiet.MaDH == order.MaDH) // Lọc theo MaDH của order
                 .Select(chiTiet => new Excel.OrderItemViewModel
                 {
                     MaDH = chiTiet.MaDH,
                     MaSp = chiTiet.MaSP,
                     TenSanPham = chiTiet.TenSP,
                     SoLuong = Convert.ToInt32(chiTiet.SoLuong),
                     Gia = Convert.ToInt32(chiTiet.GiaBan)
                 }).ToList();

                orderDetails.Add(orderDetail); // Thêm orderDetail vào danh sách orderDetails
            }
            using (var package = new ExcelPackage())
            {
                var ordersSheet = package.Workbook.Worksheets.Add("Orders");

                // Thêm tiêu đề cho ordersSheet
                ordersSheet.Cells["A1"].Value = "MaDH";
                ordersSheet.Cells["A1"].Style.Font.Bold = true;
                ordersSheet.Cells["B1"].Value = "Ho Ten";
                ordersSheet.Cells["B1"].Style.Font.Bold = true;
                ordersSheet.Cells["C1"].Value = "Số điện thoại";
                ordersSheet.Cells["C1"].Style.Font.Bold = true;
                ordersSheet.Cells["D1"].Value = "Địa Chỉ";
                ordersSheet.Cells["D1"].Style.Font.Bold = true;
                ordersSheet.Cells["E1"].Value = "Tổng tiền";
                ordersSheet.Cells["E1"].Style.Font.Bold = true;
                ordersSheet.Cells["F1"].Value = "Trạng Thái";
                ordersSheet.Cells["F1"].Style.Font.Bold = true;
                ordersSheet.Cells["G1"].Value = "Ghi Chú";
                ordersSheet.Cells["G1"].Style.Font.Bold = true;
                ordersSheet.Cells["H1"].Value = "Ngày Đặt";
                ordersSheet.Cells["H1"].Style.Font.Bold = true;
                ordersSheet.Cells["I1"].Value = "Email";
                ordersSheet.Cells["I1"].Style.Font.Bold = true;
                ordersSheet.Cells["J1"].Value = "Phương Thức Thanh Toán";
                ordersSheet.Cells["J1"].Style.Font.Bold = true;

                ordersSheet.Cells["K1"].Value = "MaSp";ordersSheet.Cells["K1"].Style.Font.Bold = true;
                ordersSheet.Cells["L1"].Value = "TenSanPham"; ordersSheet.Cells["L1"].Style.Font.Bold = true;
                ordersSheet.Cells["M1"].Value = "SoLuong"; ordersSheet.Cells["M1"].Style.Font.Bold = true;
                ordersSheet.Cells["N1"].Value = "Gia"; ordersSheet.Cells["N1"].Style.Font.Bold = true;
                // Thêm dữ liệu từ orderDetails vào Sheet Orders
                int row = 2;
                foreach (var Dataorders in orderDetails)
                {
                    ordersSheet.Cells["A" + row].Value = Dataorders.MaDH;
                    ordersSheet.Cells["B" + row].Value = Dataorders.HoTen;
                    ordersSheet.Cells["C" + row].Value = Dataorders.SoDT;
                    ordersSheet.Cells["D" + row].Value = Dataorders.DiaChi;
                    ordersSheet.Cells["E" + row].Value = Dataorders.TongTien;
                    ordersSheet.Cells["F" + row].Value = Dataorders.TrangThai;
                    ordersSheet.Cells["G" + row].Value = Dataorders.GhiChu;
                    ordersSheet.Cells["H" + row].Value = Dataorders.NgayDat;
                    ordersSheet.Cells["I" + row].Value = Dataorders.Email;
                    ordersSheet.Cells["J" + row].Value = Dataorders.PPTT;

                    int detailRow = row; // Biến tạm để duy trì chỉ số hàng của chi tiết đơn hàng
                    foreach (var detail in Dataorders.ChiTietDonHangs)
                    {
                        
                            ordersSheet.Cells["K" + detailRow].Value = detail.MaSp;
                            ordersSheet.Cells["L" + detailRow].Value = detail.TenSanPham;
                            ordersSheet.Cells["M" + detailRow].Value = detail.SoLuong;
                            ordersSheet.Cells["N" + detailRow].Value = detail.Gia;
                            // Tăng chỉ số hàng của chi tiết đơn hàng
                            detailRow++;
                    }
                    row = detailRow; // Cập nhật chỉ số hàng cho đơn hàng tiếp theo
                }




                // Trả về file Excel cho người dùng
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Orders_OrdersDetails.xlsx";
                var fileBytes = package.GetAsByteArray();

                return File(fileBytes, contentType, fileName);
            }
        }



        //public ActionResult ExportToExcel()
        //{
         

        //    using (var package = new ExcelPackage())
        //    {
        //        // Tạo một Sheet mới
        //        var ordersSheet = package.Workbook.Worksheets.Add("Orders");
        //        // Thêm tiêu đề cho ordersSheet
        //        ordersSheet.Cells["A1"].Value = "MaDH";
        //        ordersSheet.Cells["B1"].Value = "HoTen";
        //        ordersSheet.Cells["C1"].Value = "DiaChi";

        //        // Thêm dữ liệu từ orderDetails vào Sheet Orders
        //        int row = 2;
        //        foreach (var order in orders)
        //        {
        //            ordersSheet.Cells["A" + row].Value = order.MaDH;
        //            ordersSheet.Cells["B" + row].Value = order.HoTen;
        //            ordersSheet.Cells["C" + row].Value = order.DiaChi;
        //            row++;
        //        }

        //        // Trả về file Excel cho người dùng
        //        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        var fileName = "Orders_OrdersDetails.xlsx";
        //        var fileBytes = package.GetAsByteArray();

        //        return File(fileBytes, contentType, fileName);
        //    }
        //}

       
    }
}
