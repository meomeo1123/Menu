using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class DailyRevenueSummary
    {
        public DateTime Date { get; set; }
        public double? DoanhThu { get; set; }
        public double? ChiPhi { get; set; }
        public double? LoiNhuan { get; set; }
        public List<SanPham> Products { get; set; }
    }


    public class ProductInfo
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int? Quantity { get; set; }
        public float? GiaNhap { get; set; }
    }

}