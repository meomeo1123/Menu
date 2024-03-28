using System;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace tester1.Models
{
    public class MailHelper1
    {
        public static bool SendConfirmationEmail(string email, string shipName, string htmlFilePath)
        {
            using (var db = new ThucDonDataContext())
            {
                var latestOrderId = db.DonHangs.OrderByDescending(o => o.NgayDatHang).Select(o => o.MaDH).FirstOrDefault();
                var DataOrder = (from o in db.DonHangs
                                 where o.MaDH == latestOrderId
                                 select new
                                 {
                                     MaDH = o.MaDH,
                                     CustomerName = o.Ten,
                                     Address = o.DiaChi,
                                     PhoneNumber = o.Sdt,
                                     Total = o.TongTien,
                                     PaymentMethod = o.PhuongThucThanhToan,
                                 }).FirstOrDefault();

                var chiTietDonHang = (from chiTiet in db.ChiTietDonHangs
                                      where chiTiet.MaDH == DataOrder.MaDH
                                      select new
                                      {
                                          TenSanPham = chiTiet.TenSP,
                                          SoLuong = chiTiet.SoLuong,
                                          Gia = chiTiet.GiaBan.ToString()
                                      }).ToList();

                try
                {
                    var fromAddress = new MailAddress("freshop20dthe2@gmail.com", "Fresh Shop");
                    var toAddress = new MailAddress(email, shipName);
                    const string subject = "Xác nhận đơn hàng";
                    decimal totalValue = Convert.ToDecimal(DataOrder.Total);
                    string formattedTotal = totalValue.ToString("N0");
                    string productDetails = "";
                    foreach (var item in chiTietDonHang)
                    {
                        // Tạo một chuỗi HTML tương ứng với từng sản phẩm và thêm vào productDetails
                        string formattedPrice = Math.Round(Convert.ToDouble(item.Gia)).ToString("N0") + " ₫";
                        string productInfo = $@"
                        <tr>
                            <td>{item.TenSanPham}</td>
                            <td>{item.SoLuong}</td>
                            <td>{formattedPrice}</td>
                        </tr>";

                        productDetails += productInfo;
                    }
                    // Đọc nội dung từ file HTML
                    string emailContent = System.IO.File.ReadAllText(htmlFilePath);
                    emailContent = emailContent.Replace("{{shipName}}", DataOrder.CustomerName)
                                              .Replace("{{newOrderCode}}", DataOrder.MaDH)
                                              .Replace("{{district}}", DataOrder.Address)
                                              .Replace("{{mobile}}", DataOrder.PhoneNumber)
                                             .Replace("{{total}}", formattedTotal)
                                             .Replace("{{Phuongthucthanhtoan}}", DataOrder.PaymentMethod)
                                              .Replace("{{SanPham}}", productDetails);
                    var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = emailContent,
                        IsBodyHtml = true // Cho phép email chứa HTML
                    };

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("freshop20dthe2@gmail.com", "tnwt kmpj qjlq axvb")
                    };

                    smtp.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    // Thay đổi xử lý lỗi tùy thuộc vào yêu cầu của bạn
                    throw new Exception("Có lỗi xảy ra trong quá trình gửi email: " + ex.Message);
                }
            }
         

        
        }
    }
}
