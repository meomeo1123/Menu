using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Controllers
{
    public class ProductController : Controller
    {
        ThucDonDataContext data = new ThucDonDataContext();
        
        // GET: Product
        public ActionResult Index()
        {
            var all_monan = data.SanPhams.OrderBy(p => p.MaSP).ToList();
            return View(all_monan);
        }
    }
}