using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.Mvc;

namespace tester1.Models
{
    public static class AccessCounter
    {
        public static void SaveAccessDb(HttpContextBase httpContext)
        {
            if (httpContext.Request.Cookies["VisitedToday"] == null)
            {
                // Tạo cookie để đánh dấu người dùng đã truy cập trong ngày
                HttpCookie cookie = new HttpCookie("VisitedToday", "true");
                cookie.Expires = DateTime.Today.AddDays(1); // Thiết lập hết hạn vào cuối ngày

                // Thêm cookie vào Response để gửi về trình duyệt của người dùng
                httpContext.Response.Cookies.Add(cookie);

                using (var db = new ThucDonDataContext())
                {
                    DateTime today = DateTime.Today;

                    // Tìm bản ghi cho ngày hiện tại
                    var accessForToday = db.AccessCounts.FirstOrDefault(a => a.DateTime.HasValue && a.DateTime.Value.Date == today);

                    if (accessForToday != null)
                    {
                        // Nếu đã có bản ghi, tăng số lượng truy cập
                        accessForToday.TotalAcess++;
                    }
                    else
                    {
                        // Nếu chưa có, tạo mới bản ghi
                        var accessObject = new AccessCount
                        {
                            DateTime = today,
                            TotalAcess = 1 // Bắt đầu từ 1 vì đây là truy cập đầu tiên trong ngày
                        };
                        db.AccessCounts.InsertOnSubmit(accessObject);
                    }

                    // Cập nhật vào cơ sở dữ liệu
                    db.SubmitChanges();
                }
            }
        }


        // Các phương thức khác...
    }
    public class AccessCounterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AccessCounter.SaveAccessDb(filterContext.HttpContext);

            base.OnActionExecuting(filterContext);
        }
    }
}




