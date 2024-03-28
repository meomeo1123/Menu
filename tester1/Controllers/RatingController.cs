using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Controllers
{
    public class RatingController : Controller
    {
        ThucDonDataContext data = new ThucDonDataContext();
        // GET: Rating
        [Authorize]
        public ActionResult Index()
        {
            //IEnumerable<Product> listSP = (from )
            return View();
        }

        [Authorize]
        public ActionResult Create(int id, string MaDH)
        {
            var model = new RatingViewModel();
            // Code để kiểm tra xem người dùng đã đánh giá sản phẩm này chưa
            string userId = User.Identity.GetUserId();

            model.MaSP = id;
            model.MaDH = MaDH;

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(int id, string MaDH, RatingViewModel model)
        {
            model.MaSP = id;
            model.MaDH = MaDH;

            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();

                // Tạo một đánh giá mới từ dữ liệu người dùng
                var review = new DanhGia
                {
                    MaSP = model.MaSP,
                    IdUser = userId,
                    NoiDung = model.NoiDung,
                    NgayRate = DateTime.Now,
                    Rating = model.Rating,
                    Name = User.Identity.Name,
                    MaDH = model.MaDH
                };

                // Lưu đánh giá vào cơ sở dữ liệu
                data.DanhGias.InsertOnSubmit(review);
                data.SubmitChanges();

                var productToUpdate = data.ChiTietDonHangs.FirstOrDefault(p => p.MaSP == model.MaSP && p.MaDH == model.MaDH);
                if (productToUpdate != null)
                {
                    data.SubmitChanges();
                }

                // Chuyển hướng về trang chi tiết sản phẩm hoặc trang danh sách đánh giá
                return RedirectToAction("Index", "Home", new { id = model.MaSP });
            }

            // Nếu ModelState không hợp lệ, quay lại form tạo đánh giá với thông báo lỗi
            return View(model);
        }

        [Authorize]
        public ActionResult ReviewNotFound()
        {
            return View();
        }

        [Authorize]
        public ActionResult CheckReview(int MaSP, string MaDH)
        {
            string userId = User.Identity.GetUserId();

            // Kiểm tra xem đơn hàng có đánh giá từ user hiện tại không
            var existingReview = data.DanhGias.FirstOrDefault(r => r.MaSP == MaSP && r.IdUser == userId && r.MaDH == MaDH);

            if (existingReview != null)
            {
                // Đã có đánh giá từ user hiện tại cho đơn hàng này
                return RedirectToAction("Edit", "Rating", new { id = MaSP, MaDH = MaDH });
            }
            else
            {
                // Chưa có đánh giá từ user hiện tại cho đơn hàng này
                return RedirectToAction("Create", "Rating", new { id = MaSP, MaDH = MaDH });
            }
        }

        [Authorize]
        public ActionResult Edit(int id, string MaDH)
        {
            string userId = User.Identity.GetUserId();

            // Tìm đánh giá cần chỉnh sửa từ cơ sở dữ liệu dựa trên MaSP, MaDH và IdUser
            var existingReview = data.DanhGias.FirstOrDefault(r => r.MaSP == id && r.IdUser == userId && r.MaDH == MaDH);

            if (existingReview != null)
            {
                // Nếu tìm thấy đánh giá, truyền nó vào view để chỉnh sửa
                var review = new RatingViewModel
                {
                    MaSP = existingReview.MaSP,
                    MaDH = existingReview.MaDH,
                    Rating = existingReview.Rating,
                    NoiDung = existingReview.NoiDung
                    // Các trường dữ liệu khác cần thiết
                };

                return View(review);
            }
            else
            {
                // Nếu không tìm thấy đánh giá, có thể hiển thị thông báo lỗi hoặc chuyển hướng về trang khác
                return RedirectToAction("ReviewNotFound", "Rating");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, string MaDH, RatingViewModel model)
        {
            string userId = User.Identity.GetUserId();

            var existingReview = data.DanhGias.FirstOrDefault(r => r.MaSP == id && r.IdUser == userId && r.MaDH == MaDH);

            if (existingReview != null)
            {
                existingReview.NoiDung = model.NoiDung; // Cập nhật nội dung từ dữ liệu người dùng
                existingReview.Rating = model.Rating; // Cập nhật giá trị Rating từ dữ liệu người dùng

                data.SubmitChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                return RedirectToAction("Index", "Home", new { id = model.MaSP });
            }
            else
            {
                return RedirectToAction("ReviewNotFound", "Rating");
            }
        }

        public ActionResult ProductRatings(int MaSP)
        {
            // Lấy danh sách đánh giá của sản phẩm có id tương ứng từ cơ sở dữ liệu hoặc nguồn dữ liệu khác
            var ratings = data.DanhGias.Where(r => r.MaSP == MaSP).ToList();

            return PartialView("ProductRatings", ratings);
        }

        [HttpGet]
        public ActionResult FilterRatings(int MaSP, int ratingValue)
        {
            // Lọc danh sách đánh giá theo giá trị rating
            var filteredRatings = data.DanhGias.Where(r => r.MaSP == MaSP && r.Rating == ratingValue).ToList();

            return PartialView("FilterRatings", filteredRatings);
        }


        //public ActionResult ProductRatingsPartial(int MaSP)
        //{
        //    // Lấy danh sách đánh giá từ database hoặc từ nguồn dữ liệu phù hợp
        //    var allRatings = data.DanhGias.Where(r => r.MaSP == MaSP).ToList();

        //    return PartialView("ProductRatingsPartial", allRatings);
        //}
        public ActionResult ProductRatingsPartial(int MaSP, int pageNumber = 1)
        {
            var pageSize = 3; // Số lượng mục trên mỗi trang
            var allRatings = data.DanhGias.Where(r => r.MaSP == MaSP).ToList();

            var paginatedRatings = allRatings.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return PartialView("ProductRatingsPartial", paginatedRatings);
        }





    }
}