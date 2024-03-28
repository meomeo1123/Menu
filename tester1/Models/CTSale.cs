using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class CTSale
    {
        public AspNetUser AspNetUser { get; set; }
        public KhuyenMai KhuyenMai { get; set; }
        public int Id { get; set; }
        public ChiTietSale ChiTietSales { get; set; }
        public int IdKm { get; set; }
        public string IdUser { get; set; }
    }
}