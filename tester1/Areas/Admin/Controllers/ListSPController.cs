using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Areas.Admin.Controllers
{
    public class ListSPController : Controller
    {
        // GET: Admin/ListSP
        ThucDonDataContext data = new ThucDonDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? categoryId)
        {
            ViewBag.Categories = data.DanhMucs.ToList();

            if (categoryId.HasValue)
            {
                if (categoryId.Value != 0)
                { var products = data.SanPhams.Include(p => p.DanhMuc)
                   
                        .Where(p => p.MaDM == categoryId.Value)
                        .ToList();
                    return View(products);
                }
                else
                {
                    var products = (from p in data.SanPhams
                                    join s in data.TrangThais on p.IdS equals s.IdS
                                    join c in data.DanhMucs on p.MaDM equals c.MaDM
                                    select new Product
                                    {
                                        NoiDung = p.NoiDung,
                                        MaDM = c.MaDM,
                                        MaSP = p.MaSP,
                                        TenSP = p.TenSP,
                                        GiaBan = ((float)p.GiaBan.Value),
                                        MoTa = p.MoTa,
                                        Hinh = p.HinhAnh,
                                        TenDM = c.TenDM,
                                        Status = s.Status
                                    }
                                ).ToList();

                    return View(products);
                }
            }
            else
            {
                var products = (from p in data.SanPhams
                                join s in data.TrangThais on p.IdS equals s.IdS
                                join c in data.DanhMucs on p.MaDM equals c.MaDM
                                select new Product
                                {
                                    MaDM = c.MaDM,
                                    MaSP = p.MaSP,
                                    TenSP = p.TenSP,
                                    GiaBan = ((float)p.GiaBan.Value),
                                    GiaNhap = ((float)p.GiaNhap.Value),
                                    MoTa = p.MoTa,
                                    Hinh = p.HinhAnh,
                                    TenDM = c.TenDM,
                                    Status = s.Status
                                }
                            ).ToList();

                return View(products);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            // Lấy danh sách danh mục
            Product sanPham = new Product();
            var categories = data.DanhMucs.ToList();
            var statusList = data.TrangThais.ToList();
            // Tạo SelectList cho danh sách danh mục
            ViewBag.Categories = new SelectList(data.DanhMucs, "MaDM", "TenDM");
            ViewBag.StatusList = new SelectList(data.TrangThais, "IdS", "Status");
            return View(sanPham);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(Product product)
        {
            try
            {
                if (string.IsNullOrEmpty(product.MaDM.ToString()) || string.IsNullOrEmpty(product.IdS.ToString()))
                {
                    ModelState.AddModelError("", "Vui lòng chọn cả danh mục và trạng thái sản phẩm.");
                }
                if (ModelState.IsValid)
                {
                    if (product.MaDM == null || product.IdS == null)
                    {
                        throw new Exception("Vui lòng chọn danh mục và trạng thái sản phẩm.");
                    }
                    using (var db = new ThucDonDataContext())
                    {
                        var sanpham = new SanPham()
                        {
                            MaSP = product.MaSP,
                            MaDM = product.MaDM,
                            TenSP = product.TenSP,
                            GiaBan = product.GiaBan,
                            GiaNhap = product.GiaNhap,
                            MoTa = product.MoTa,
                            NoiDung = product.NoiDung,
                            HinhAnh = product.Hinh,
                            IdS = product.IdS
                        };
                        db.SanPhams.InsertOnSubmit(sanpham);
                        db.SubmitChanges();
                    }
                    product.GiaNhap = float.Parse(product.GiaNhap.ToString().Replace(",", ""));
                    product.GiaBan = float.Parse(product.GiaBan.ToString().Replace(",", ""));
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }

        //---------edit-------------
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var E_sanpham = data.SanPhams.First(m => m.MaSP == id);
            var categories = data.DanhMucs.ToList();
            var statusList = data.TrangThais.ToList();
            // Tạo SelectList cho danh sách danh mục
            ViewBag.Categories = new SelectList(categories, "MaDM", "TenDM");
            ViewBag.StatusList = new SelectList(statusList, "IdS", "Status");
            return View(E_sanpham);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_sanpham = data.SanPhams.First(m => m.MaSP == id);
            var E_MaDanhMuc = Convert.ToInt32(collection["MaDanhMuc"]);
            var E_TenSP = collection["TenSP"];
            var E_GiaBan = Convert.ToInt32(collection["GiaBan"]);
            var E_GiaNhap = Convert.ToInt32(collection["GiaNhap"]);
            var E_Mota = collection["MoTa"];
            var E_NoiDung = collection["NoiDung"];
            var E_HinhAnh = collection["HinhAnh"];
            var E_IdS = Convert.ToInt32(collection["IdS"]);
            E_sanpham.MaSP = id;
            if (string.IsNullOrEmpty(E_TenSP))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_sanpham.MaDM = E_MaDanhMuc;
                E_sanpham.TenSP = E_TenSP;
                E_sanpham.GiaBan = E_GiaBan;
                E_sanpham.GiaNhap = E_GiaNhap;
                E_sanpham.MoTa = E_Mota;
                E_sanpham.NoiDung = E_NoiDung;
                E_sanpham.HinhAnh = E_HinhAnh;
                E_sanpham.IdS = E_IdS;
                UpdateModel(E_sanpham);
                data.SubmitChanges();
                return RedirectToAction("Index");
            }
            return this.Edit(id);
        }

        ///---------detete-----------
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var E_sanpham = data.SanPhams.SingleOrDefault(m => m.MaSP == id);
            return View(E_sanpham);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var categories = data.DanhMucs.ToList();
            var statusList = data.TrangThais.ToList();
            // Tạo SelectList cho danh sách danh mục
            ViewBag.Categories = new SelectList(categories, "MaDM", "TenDM");
            ViewBag.StatusList = new SelectList(statusList, "IdS", "Status");
            var sanPham = data.SanPhams.SingleOrDefault(m => m.MaSP == id);
            data.SanPhams.DeleteOnSubmit(sanPham);
            data.SubmitChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/img/" + file.FileName));
            return "/Content/img/" + file.FileName;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult UpdateStatus(int id, int trangThai)
        {
            var product = data.SanPhams.FirstOrDefault(s =>s.MaSP == id);

            if (product == null)
            {
                return Json(new { success = false });
            }

            product.IdS = trangThai;
            data.SubmitChanges();

            return Json(new { success = true });
        }


    }
}