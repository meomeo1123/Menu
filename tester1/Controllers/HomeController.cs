using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Controllers
{
    public class HomeController : Controller
    {
        ThucDonDataContext data = new ThucDonDataContext();
        public ActionResult Index(string SearchString = "")
        {
            if (SearchString != "")
            {
                ViewBag.ActivePage = "Index";
                var SearchsanPham = data.SanPhams.Include(s => s.MaSP).Where(x => x.TenSP.ToUpper().Contains(SearchString.ToUpper()));
                ViewBag.active = 0; // Không có mục active khi tìm kiếm
                return View(SearchsanPham.ToList());
            }
            var sanPham = data.SanPhams.Include(s => s.MaSP);
            var danhMucs = sanPham.Select(s => s.DanhMuc).Distinct().ToList();
            ViewBag.DanhMucs = danhMucs;
            return View(sanPham.ToList());
        }

        public ActionResult _Navbar()
        {
            var categories = data.DanhMucs.ToList();
            ViewBag.Categories = categories;
            return PartialView("_Navbar");
        }
        public ActionResult ProductCategory(int id)
        {
            var listProduct = data.SanPhams.Where(s => s.MaDM == id).ToList();
            var firstProduct = listProduct.FirstOrDefault();
            ViewBag.FirstProduct = firstProduct;
            return View(listProduct);
        }
        public ActionResult DetailProduct(int id)
        {
            var detailProduct = data.SanPhams.SingleOrDefault(s => s.MaSP == id);
            return View(detailProduct);
        }

    }
}
