using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class DesktopPushApiController : ControllerBase
    {
        private readonly ThongBao_DAL thongBaoDAL;

        public DesktopPushApiController(ThongBao_DAL thongBaoDAL)
        {
            this.thongBaoDAL = thongBaoDAL;
        }

        private int? GetCurrentUserId()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
                return null;
            return maTaiKhoan.Value;
        }

        /// <summary>
        /// Lấy tối đa 5 thông báo chưa đọc chưa từng được đẩy desktop
        /// GET /api/notifications/unread-for-push
        /// </summary>
        [HttpGet("unread-for-push")]
        public IActionResult GetUnreadForPush()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });

            var items = thongBaoDAL.LayThongBaoChuaDocChoDesktopPush(userId.Value, 5);

            System.Console.WriteLine($"[DesktopPushApi] unread-for-push userId={userId}, count={items.Count}");

            var result = items.Select(i => new
            {
                id = i.Id,
                title = i.Title,
                message = i.Message,
                category = GetCategoryFromTitle(i.Title),
                url = "/ThongBao/Index",
                createdAt = i.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
            }).ToList();

            return Ok(new { success = true, items = result });
        }

        /// <summary>
        /// Đánh dấu các thông báo đã được đẩy desktop
        /// POST /api/notifications/mark-desktop-pushed
        /// </summary>
        [HttpPost("mark-desktop-pushed")]
        public IActionResult MarkDesktopPushed([FromBody] MarkDesktopPushedRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });

            if (request?.NotificationIds == null || request.NotificationIds.Count == 0)
                return Ok(new { success = true, marked = 0 });

            thongBaoDAL.MarkDesktopPushed(userId.Value, request.NotificationIds);

            System.Console.WriteLine($"[DesktopPushApi] mark-desktop-pushed userId={userId}, ids=[{string.Join(",", request.NotificationIds)}]");

            return Ok(new { success = true, marked = request.NotificationIds.Count });
        }

        private static string GetCategoryFromTitle(string title)
        {
            if (title.Contains("Quá hạn")) return "overdue";
            if (title.Contains("đến lịch") || title.Contains("Hôm nay")) return "upcoming";
            return "updated";
        }
    }

    public class MarkDesktopPushedRequest
    {
        public List<int> NotificationIds { get; set; } = new();
    }
}
