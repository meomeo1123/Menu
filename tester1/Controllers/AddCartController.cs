using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Microsoft.AspNet.Identity;
using mvcDangNhap.common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tester1.Models;
using Google.Apis.Gmail.v1.Data;
using System.IO;
using System.Net.Mail;
using System.Net;
using MoMo;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace tester1.Controllers
{
    public class AddCartController : Controller
    {
        // GET: AddCart
        ThucDonDataContext db = new ThucDonDataContext();
        private const string CartSession = "CartSession";

        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(long id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.product.MaSP == id);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public ActionResult Update_Quantity_Cart(FormCollection form)
        {
            var cart = Session[CartSession] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session[CartSession] = cart;
            }

            int id_pro = int.Parse(form["ID_Product"]);
            int quantity = int.Parse(form["Quantity"]);

            // Cập nhật số lượng trong giỏ hàng
            var item = cart.SingleOrDefault(x => x.product.MaSP == id_pro);
            if (item != null)
            {
                item.Quantity = quantity;
            }

            return RedirectToAction("Index", "AddCart");
        }

        public ActionResult AddItem(long productId, int quantity)
        {

            var product = db.SanPhams.FirstOrDefault(c => c.MaSP == productId);
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.product.MaSP == productId))
                {

                    foreach (var item in list)
                    {
                        if (item.product.MaSP == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới đối tượng cart item
                    var item = new CartItem();
                    item.product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                //tạo mới đối tượng cart item
                var item = new CartItem();
                item.product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                //Gán vào session
                Session[CartSession] = list;
            }
            return RedirectToAction("Index");
        }

        public PartialViewResult BagCart()
        {
            int total_item = 0;
            var cart = Session[CartSession] as List<CartItem>;
            if (cart != null)
                total_item = cart.Sum(item => item.Quantity);
            ViewBag.QuantityCart = total_item;
            return PartialView("BagCart");
        }
        public ActionResult UpdateCartItemCount()
        {
            var cart = Session[CartSession] as List<CartItem>;
            int itemCount = cart?.Sum(item => item.Quantity) ?? 0;

            return Content(itemCount.ToString());
        }
        private List<CartItem> GetCartItems()
        {
            var cart = Session[CartSession] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session[CartSession] = cart;
            }
            return cart;
        }
        private string GenerateOrderCode(string maxOrderCode)
        {
            string orderCodePrefix = "DH";
            int orderNumber = int.Parse(maxOrderCode.Substring(2)) + 1;

            // Định dạng mã đơn hàng với 2 chữ số
            string orderCode = orderCodePrefix + orderNumber.ToString("D2");

            return orderCode;
        }
        [HttpGet]
        public ActionResult Payment()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Payment", "AddCart") });
            }
            else
            {
                var cart = Session[CartSession];
                var list = new List<CartItem>();
                if (cart != null)
                {
                    list = (List<CartItem>)cart;
                }
                return View(list);
            }
        }
        private EmailService emailService;
        public AddCartController()
        {
            emailService = new EmailService();
        }


        // sale 
        // public ActionResult GetSalesForUser()
        // {
        //     string loggedInUserId = User.Identity.GetUserId(); // Lấy thông tin id của người dùng đăng nhập ở đây
        //     var userSales = db.ChiTietSales
        //         .Where(sale => sale.idUser == loggedInUserId)
        //        .Join(db.KhuyenMais,
        //               sale => sale.IdKm,
        //              km => km.IdKm,
        //               (sale, km) => new { ChiTietSale = sale, KhuyenMai = km })
        //        .ToList();
        //       return Json(userSales, JsonRequestBehavior.AllowGet);
        // }

        public PartialViewResult GetSalesForUser()
        {
            string loggedInUserId = User.Identity.GetUserId();
            var userSales = db.ChiTietSales
                .Where(sale => sale.idUser == loggedInUserId)
                .Join(db.KhuyenMais,
                      sale => sale.IdKm,
                      km => km.IdKm,
                      (sale, km) => new SalePromotionDTO
                      {
                          ChiTietSale = sale,
                          KhuyenMai = km
                      })
                .ToList();
            return PartialView(userSales);
        }

        [HttpGet]
        public ActionResult GetPromoSale()
        {
            // Lấy giá trị promoSale từ Session
            var PromoSale = Convert.ToInt32(Session["SubmittedPromoSale"]);
            return Content(PromoSale.ToString()); // Trả về giá trị giảm giá dưới dạng chuỗi
        }


        [HttpPost]
        public ActionResult SubmitPromoSale(int promoSale, int promoId)
        {
            // Lưu promoSale vào session
            Session["SubmittedPromoSale"] = promoSale;
            Session["IdKm"] = promoId;
            return Json(new { success = true });
        }



        // end 
        [HttpPost]
        public ActionResult Payment(string shipName, string mobile, string address, string email, string district, string note, string paymentMethod)
        {
            var userId = User.Identity.GetUserId();
            var code = new { Success = false, Code = -1 };
            var maxOrderCode = db.DonHangs.Max(o => o.MaDH);
            string newOrderCode = GenerateOrderCode(maxOrderCode);
            var order = new DonHang();
            order.MaDH = newOrderCode;
            order.NgayDatHang = DateTime.Now;
            Session["diachi"] = order.DiaChi = address;
            order.Sdt = mobile;
            order.Ten = shipName;
            order.Email = email;
            order.Ghichu = note;
            order.Quan = district;
            order.TrangThaiDonHang = 1;
            order.IdUser = userId;
            TempData["Email"] = email;
            TempData["MaDH"] = newOrderCode;
            // truy xuất sale

            // end
            if (paymentMethod == "1")
            {
                order.PhuongThucThanhToan = "Tiền mặt";
                db.DonHangs.InsertOnSubmit(order);
                db.SubmitChanges();

                var id = order.MaDH;
                var cart = (List<CartItem>)Session[CartSession];
                decimal total = 0;
                try
                {
                    foreach (var item in cart)
                    {
                        var orderDetail = new ChiTietDonHang();
                        orderDetail.MaSP = item.product.MaSP;
                        orderDetail.MaDH = id;
                        orderDetail.TenSP = item.product.TenSP;
                        orderDetail.GiaBan = item.product.GiaBan;
                        orderDetail.SoLuong = item.Quantity;
                        db.ChiTietDonHangs.InsertOnSubmit(orderDetail);
                        total += (decimal)(item.product.GiaBan.GetValueOrDefault(0) * item.Quantity);
                    }

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();
                    order.TongTien = (double)total;
                    db.SubmitChanges();

                    var Sale = Convert.ToInt32(Session["SubmittedPromoSale"]);
                    if (Sale > 0)
                    {
                        // Lấy MaDH từ TempData
                        var maDH = TempData["MaDH"] as string;
                        if (maDH != null)
                        {
                            var Order = db.DonHangs.FirstOrDefault(o => o.MaDH == maDH);
                            if (Order != null)
                            {
                                // Sử dụng thông tin trong order để tính toán giảm giá và cập nhật tổng tiền
                                Order.TongTien = Order.TongTien - (Order.TongTien * Sale / 100.0);
                                Order.IdKm = Convert.ToInt32(Session["IdKm"]);
                                var chiTietSale = db.ChiTietSales.FirstOrDefault(c => c.idUser == userId && c.IdKm == Order.IdKm);
                                chiTietSale.Status = 0;
                                // 0 là dã sử dụng 
                                // Lưu các thay đổi vào cơ sở dữ liệu
                                db.SubmitChanges();
                            }
                        }
                    }
                    // Xóa giỏ hàng
                    Session[CartSession] = null;
                    Session["IdKm"] = null;
                    Session["SubmittedPromoSale"] = null;
                    // Gửi email xác nhận cho khách hàng
                    try
                    {
                        string emailContentPath = Server.MapPath("~/Content/template/send2.html");
                        string emailContent = System.IO.File.ReadAllText(emailContentPath);

                        // Thực hiện thay thế các placeholder trong file HTML với dữ liệu từ OrderEmailModel
                        emailContent = emailContent.Replace("{{shipName}}", shipName)
                                                   .Replace("{{newOrderCode}}", newOrderCode)
                                                   //.Replace("{{NgayDatHang}}", DateTime.Now)
                                                   .Replace("{{district}}", district)
                                                   .Replace("{{mobile}}", mobile)
                                                   .Replace("{{total}}", total.ToString())
                                                   .Replace("{{email}}", email);
                        bool isEmailSent = MailHelper1.SendConfirmationEmail(email, shipName, emailContentPath);

                        if (isEmailSent)
                        {
                            TempData["SuccessMessage"] = "Đơn hàng đã được đặt thành công và email xác nhận đã được gửi.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình gửi email.";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình gửi email: " + ex.Message;
                    }

                    // Gửi email thông báo đơn hàng mới cho admin

                    TempData["Email"] = email;
                    return RedirectToAction("Success", new { email = email });
                }
                catch (Exception ex)
                {
                    // Ghi log hoặc xử lý lại ngoại lệ
                    // ...

                    // Bạn có thể thực hiện một hành động khác hoặc báo lỗi cho người dùng tại đây
                    return Redirect("/AddCart/UnSuccess");
                }
            }
            else if (paymentMethod == "2")
            {
                return RedirectToAction("CheckOut", new { shipName = shipName, mobile = mobile, district = district, address = address, email = email, note = note, paymentMethod = paymentMethod });
            }
            else if (paymentMethod == "3")
            {
                Session["User"] = userId;
                return RedirectToAction("PaymentMomo", new { shipName = shipName, mobile = mobile, district = district, address = address, email = email, note = note, paymentMethod = paymentMethod });
            }

            // ...


            return Redirect("/AddCart/Success");
        }

        public ActionResult Success(string email)
        {
            // Lấy giá trị email từ TempData
            TempData["Email"] = email;
            return View();
        }

        public ActionResult UnSuccess()
        {
            return View();
        }
        // vnpay
        public ActionResult CheckOut(string shipName, string mobile, string address, string email, string district, string note, string paymentMethod)
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            decimal tongtien = 0;
            var Sale = Convert.ToInt32(Session["SubmittedPromoSale"]);
            if (Sale > 0)
            {
                // Lấy MaDH từ TempData
                foreach (var item in list)
                {
                    decimal thanhtien = item.Quantity * (decimal)item.product.GiaBan;
                    thanhtien = thanhtien - (thanhtien * (decimal)Sale / 100.0m);
                    tongtien += thanhtien;
                }
            }
            else
            {
                foreach (var item in list)
                {
                    decimal thanhtien = item.Quantity * (decimal)item.product.GiaBan;
                    tongtien += thanhtien;
                }
            }

            tongtien = tongtien * 100;
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", tongtien.ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 100000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            TempData["ShipName"] = shipName;
            TempData["Mobile"] = mobile;
            TempData["Address"] = address;
            TempData["Email"] = email;
            TempData["Note"] = note;
            TempData["District"] = district;

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?
                try
                {
                    if (checkSignature)
                    {
                        if (vnp_ResponseCode == "00")
                        {
                            //Thanh toán thành công
                            ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                            string shipName = TempData["ShipName"] as string;
                            string mobile = TempData["Mobile"] as string;
                            string address = TempData["Address"] as string;
                            string email = TempData["Email"] as string;
                            string note = TempData["Note"] as string;
                            string district = TempData["District"] as string;
                            var userId = User.Identity.GetUserId();
                            var code = new { Success = false, Code = -1 };
                            var maxOrderCode = db.DonHangs.Max(o => o.MaDH);
                            string newOrderCode = GenerateOrderCode(maxOrderCode);
                            var order = new DonHang();
                            order.MaDH = newOrderCode;
                            order.NgayDatHang = DateTime.Now;
                            order.DiaChi = address;
                            order.Sdt = mobile;
                            order.Ten = shipName;
                            order.Email = email;
                            order.Ghichu = note;
                            order.Quan = district;
                            order.TrangThaiDonHang = 1;
                            order.IdUser = userId;
                            order.PhuongThucThanhToan = "VNPAY";
                            db.DonHangs.InsertOnSubmit(order);
                            db.SubmitChanges();

                            var id = order.MaDH;

                            // Lưu chi tiết đơn hàng vào cơ sở dữ liệu
                            var cart = (List<CartItem>)Session[CartSession];
                            decimal total = 0;

                            foreach (var item in cart)
                            {
                                var orderDetail = new ChiTietDonHang();
                                orderDetail.MaSP = item.product.MaSP;
                                orderDetail.MaDH = id;
                                orderDetail.TenSP = item.product.TenSP;
                                orderDetail.GiaBan = item.product.GiaBan;
                                orderDetail.SoLuong = item.Quantity;

                                db.ChiTietDonHangs.InsertOnSubmit(orderDetail);

                                total += (decimal)(item.product.GiaBan.GetValueOrDefault(0) * item.Quantity);
                            }
                            db.SubmitChanges();

                            // Cập nhật tổng tiền của đơn hàng
                            order.TongTien = (double)total;
                            db.SubmitChanges();

                            // sale
                            var Sale = Convert.ToInt32(Session["SubmittedPromoSale"]);
                            if (Sale > 0)
                            {
                                // Lấy MaDH từ TempData
                                var maDH = TempData["MaDH"] as string;
                                if (maDH != null)
                                {
                                    var Order = db.DonHangs.FirstOrDefault(o => o.MaDH == maDH);
                                    if (Order != null)
                                    {
                                        // Sử dụng thông tin trong order để tính toán giảm giá và cập nhật tổng tiền
                                        Order.TongTien = Order.TongTien - (Order.TongTien * Sale / 100.0);
                                        Order.IdKm = Convert.ToInt32(Session["IdKm"]);
                                        var chiTietSale = db.ChiTietSales.FirstOrDefault(c => c.idUser == userId && c.IdKm == Order.IdKm);
                                        chiTietSale.Status = 0;

                                        //0 là voucher đã sử dụng
                                        // Lưu các thay đổi vào cơ sở dữ liệu
                                        db.SubmitChanges();
                                    }
                                }
                            }

                            Session[CartSession] = null;
                            Session["IdKm"] = null;
                            Session["SubmittedPromoSale"] = null;

                            // Gửi thông báo thành công cho người dùng
                            // Gửi email xác nhận cho khách hàng
                            try
                            {
                                string emailContentPath = Server.MapPath("~/Content/template/send2.html");
                                string emailContent = System.IO.File.ReadAllText(emailContentPath);

                                // Thực hiện thay thế các placeholder trong file HTML với dữ liệu từ OrderEmailModel
                                emailContent = emailContent.Replace("{{shipName}}", shipName)
                                                           .Replace("{{newOrderCode}}", newOrderCode)
                                                           //.Replace("{{NgayDatHang}}", DateTime.Now)
                                                           .Replace("{{district}}", district)
                                                           .Replace("{{mobile}}", mobile)
                                                           .Replace("{{total}}", total.ToString())
                                                           .Replace("{{email}}", email);
                                bool isEmailSent = MailHelper1.SendConfirmationEmail(email, shipName, emailContentPath);

                                if (isEmailSent)
                                {
                                    TempData["SuccessMessage"] = "Đơn hàng đã được đặt thành công và email xác nhận đã được gửi.";
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình gửi email.";
                                }
                            }
                            catch (Exception ex)
                            {
                                TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình gửi email: " + ex.Message;
                            }

                            // end mail
                            ViewBag.Message = "Thanh toán thành công hóa đơn " + order.MaDH;
                            return Redirect("/AddCart/Success");
                        }
                        else if (vnp_ResponseCode == "24")
                        {
                            // Thanh toán bị huỷ
                            // ...
                            ViewBag.Message = "Thanh toán thất bại";
                        }
                        else
                        {
                            //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                            ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                    return View("ErrorView");
                }


            }
            return View();
        }
        // end vnpay

        // momo tới chơi bro

        public ActionResult PaymentMomo(string shipName, string mobile, string address, string email, string district, string note, string paymentMethod)
        {

            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            decimal tongtien = 0;
            var Sale = Convert.ToInt32(Session["SubmittedPromoSale"]);
            if (Sale > 0)
            {
                // Lấy MaDH từ TempData
                foreach (var item in list)
                {
                    decimal thanhtien = item.Quantity * (decimal)item.product.GiaBan;
                    thanhtien = thanhtien - (thanhtien * (decimal)Sale / 100.0m);
                    tongtien += thanhtien;
                }
            }
            else
            {
                foreach (var item in list)
                {
                    decimal thanhtien = item.Quantity * (decimal)item.product.GiaBan;
                    tongtien += thanhtien;
                }
            }
            tongtien = tongtien * 1;
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "test";
            string returnUrl = "https://localhost:44342/AddCart/ConfirmPaymentClient";
            string notifyurl = " https://9ce7-116-100-43-50.ngrok-free.app/AddCart/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = tongtien.ToString();
            string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            TempData["ShipName"] = shipName;
            TempData["Mobile"] = mobile;
            TempData["Address"] = address;
            TempData["Email"] = email;
            TempData["Note"] = note;
            TempData["District"] = district;

            return Redirect(jmessage.GetValue("payUrl").ToString());


        }
        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        public ActionResult ConfirmPaymentClient(Result result)
        {
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            ViewBag.mess = rErrorCode;
            SavePayment();
            return View();
        }

        public void SavePayment()
        {
            if (Request.QueryString.Count > 0)
            {
                string errorCode = Request.QueryString["errorCode"];

                if (errorCode == "0")
                {
                    //  Giao dịch thanh toán thành công
                    ViewBag.Message = "Giao dịch thanh toán thành công";
                    string shipName = TempData["ShipName"] as string;
                    string mobile = TempData["Mobile"] as string;
                    string address = TempData["Address"] as string;
                    string email = TempData["Email"] as string;
                    string note = TempData["Note"] as string;
                    string district = TempData["District"] as string;

                    var code = new { Success = false, Code = -1 };
                    var maxOrderCode = db.DonHangs.Max(o => o.MaDH);
                    string newOrderCode = GenerateOrderCode(maxOrderCode);
                    var order = new DonHang();
                    order.MaDH = newOrderCode;
                    order.NgayDatHang = DateTime.Now;
                    order.DiaChi = address;
                    order.Sdt = mobile;
                    order.Ten = shipName;
                    order.Email = email;
                    order.Ghichu = note;
                    order.Quan = district;
                    order.TrangThaiDonHang = 1;
                    if (Session["User"] != null)
                    {
                        order.IdUser = Session["User"].ToString();
                    }
                    order.PhuongThucThanhToan = "MoMo";
                    db.DonHangs.InsertOnSubmit(order);
                    db.SubmitChanges();

                    var id = order.MaDH;

                    // Lưu chi tiết đơn hàng vào cơ sở dữ liệu
                    var cart = (List<CartItem>)Session[CartSession];
                    decimal total = 0;

                    foreach (var item in cart)
                    {
                        var orderDetail = new ChiTietDonHang();
                        orderDetail.MaSP = item.product.MaSP;
                        orderDetail.MaDH = id;
                        orderDetail.TenSP = item.product.TenSP;
                        orderDetail.GiaBan = item.product.GiaBan;
                        orderDetail.SoLuong = item.Quantity;

                        db.ChiTietDonHangs.InsertOnSubmit(orderDetail);

                        total += (decimal)(item.product.GiaBan.GetValueOrDefault(0) * item.Quantity);
                    }
                    db.SubmitChanges();

                    // Cập nhật tổng tiền của đơn hàng
                    order.TongTien = (double)total;
                    db.SubmitChanges();

                    // sale
                    var Sale = Convert.ToInt32(Session["SubmittedPromoSale"]);
                    if (Sale > 0)
                    {
                        // Lấy MaDH từ TempData
                        var maDH = TempData["MaDH"] as string;
                        if (maDH != null)
                        {
                            var Order = db.DonHangs.FirstOrDefault(o => o.MaDH == maDH);
                            if (Order != null)
                            {
                                // Sử dụng thông tin trong order để tính toán giảm giá và cập nhật tổng tiền
                                Order.TongTien = Order.TongTien - (Order.TongTien * Sale / 100.0);
                                Order.IdKm = Convert.ToInt32(Session["IdKm"]);
                                var chiTietSale = db.ChiTietSales.FirstOrDefault(c => c.idUser == Session["User"] && c.IdKm == Order.IdKm);
                                chiTietSale.Status = 0;

                                //0 là voucher đã sử dụng
                                // Lưu các thay đổi vào cơ sở dữ liệu
                                db.SubmitChanges();
                            }
                        }
                    }
                    Session[CartSession] = null;
                    Session["User"] = null;
                    Session["IdKm"] = null;
                    Session["SubmittedPromoSale"] = null;
                    // Gửi email xác nhận cho khách hàng
                    try
                    {
                        string emailContentPath = Server.MapPath("~/Content/template/send2.html");
                        string emailContent = System.IO.File.ReadAllText(emailContentPath);

                        // Thực hiện thay thế các placeholder trong file HTML với dữ liệu từ OrderEmailModel
                        emailContent = emailContent.Replace("{{shipName}}", shipName)
                                                   .Replace("{{newOrderCode}}", newOrderCode)
                                                   //.Replace("{{NgayDatHang}}", DateTime.Now)
                                                   .Replace("{{district}}", district)
                                                   .Replace("{{mobile}}", mobile)
                                                   .Replace("{{total}}", total.ToString())
                                                   .Replace("{{email}}", email);
                        bool isEmailSent = MailHelper1.SendConfirmationEmail(email, shipName, emailContentPath);

                        if (isEmailSent)
                        {
                            TempData["SuccessMessage"] = "Đơn hàng đã được đặt thành công và email xác nhận đã được gửi.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình gửi email.";
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình gửi email: " + ex.Message;
                    }

                    //  end mail
                    ViewBag.Message = "Thanh toán thành công hóa đơn " + order.MaDH;
                }
                else
                {
                    // Giao dịch thanh toán thất bại
                    ViewBag.Message = "Giao dịch thanh toán thất bại. Mã lỗi: " + errorCode;
                    // Thực hiện các hành động khác khi thanh toán thất bại ở đây
                }
            }

        }
    }
}

