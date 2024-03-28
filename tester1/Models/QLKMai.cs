using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class QLKMai
    {
        public KhuyenMai KhuyenMai { get; set; }
        public string Status { get; set; }
        public QuanLiKM QlKm { get; set; }
        public bool IsSaved { get; set; }
        public ChiTietSale ChiTietSale { get; set; }

    }
}