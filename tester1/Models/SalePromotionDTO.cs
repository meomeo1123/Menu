using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class SalePromotionDTO
    {
        public ChiTietSale ChiTietSale { get; set; }
        public AspNetUser User { get; set; }
        public KhuyenMai KhuyenMai { get; set; }

        public List<KhuyenMai> SalesData { get; set; }
    }
}