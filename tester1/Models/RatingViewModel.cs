using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tester1.Models
{
    public class RatingViewModel
    {
        public int IdRate { get; set; }
        public int MaSP { get; set; }
        public string IdUser { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayRate { get; set; }
        public int Rating { get; set; }
        public string Name { get; set; }
        public string MaDH { get; set; }
    }
}