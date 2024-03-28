using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class Excel
    {
        public class OrderDetailViewModel
        {
            public string MaDH { get; set; }
            public string HoTen { get; set; }
            public string DiaChi { get; set; }
            public string SoDT { get; set; }
            public decimal TongTien { get; set; }
            public int TrangThai { get; set; }
            public string GhiChu { get; set; }
            public string NgayDat { get; set; }
            public string Email { get; set; }
            public DateTime? NgayHuy { get; set; }
            public string PPTT { get; set; }
            public List<OrderItemViewModel> ChiTietDonHangs { get; set; }
        }
        public class OrderItemViewModel
        {
            public string MaDH { get; set; }
            public int MaSp { get; set; }
            public string TenSanPham { get; set; }
            public int SoLuong { get; set; }
            public decimal Gia { get; set; }
        }
    }
}
