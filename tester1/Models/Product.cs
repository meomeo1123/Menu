using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class Product
    {
        public int MaSP { get; set; }
        public int MaDM { get; set; }
        public string TenDM { get; set; }
        public string Hinh { get; set; }
        public string TenSP { get; set; }
        public float GiaBan { get; set; }
        public float GiaNhap { get; set; }
        public string NoiDung { get; set; }
        public string MoTa { get; set; }
        public int IdS { get; set; }
        public string Status { get; set; }
        public IEnumerable<TrangThai> s_status { get; set; }
        public IEnumerable<DanhMuc> d_danhmuc { get; set; }
     
    }
}