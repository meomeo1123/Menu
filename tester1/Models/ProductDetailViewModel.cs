using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class ProductDetailViewModel
    {
        public SanPhamViewModel Product { get; set; }
        public List<RatingViewModel> Ratings { get; set; }
        public DanhMucViewModel DanhMuc { get; set; }
    }
}