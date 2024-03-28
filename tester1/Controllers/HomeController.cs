using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Controllers
{
    public class HomeController : Controller
    {
        ThucDonDataContext data = new ThucDonDataContext();
        [AccessCounter]
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
        //public ActionResult DetailProduct(int id)
        //{
        //    var detailProduct = data.SanPhams.SingleOrDefault(s => s.MaSP == id);
        //    return View(detailProduct);
        //}
        public ActionResult DetailProduct(int id)
        {
            var product = data.SanPhams.SingleOrDefault(s => s.MaSP == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            //var ratings = data.DanhGias.Where(r => r.MaSP == id).ToList();
            var ratings = data.DanhGias
            .Where(r => r.MaSP == id)
            .Select(dg => new RatingViewModel
            {
                IdRate = dg.IdRate,
                MaSP = dg.MaSP,
                IdUser = dg.IdUser,
                NoiDung = dg.NoiDung,
                NgayRate = (DateTime)dg.NgayRate,
                Rating = (int)dg.Rating,
                Name = dg.AspNetUser.Name
            })
            .ToList();

            var viewModel = new ProductDetailViewModel
            {
                Product = new SanPhamViewModel
                {
                    MaSP = product.MaSP,
                    MaDM = product.MaDM,
                    HinhAnh = product.HinhAnh,
                    TenSP = product.TenSP,
                    GiaBan = (float)product.GiaBan,
                    GiaNhap = (float)product.GiaNhap,
                    NoiDung = product.NoiDung,
                    MoTa = product.MoTa,
                    IdS = (int)product.IdS
                },
                Ratings = ratings
            };

            return View("DetailProduct", viewModel);
        }

    }
}
