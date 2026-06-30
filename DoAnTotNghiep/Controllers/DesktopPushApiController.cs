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

        [HttpGet("unread-for-push")]
        // lấy thông báo chưa đọc để đẩy desktop
        public IActionResult GetUnreadForPush()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });

            var items = thongBaoDAL.LayThongBaoChuaDocChoDesktopPush(userId.Value, 50);

            var result = items.Select(i => new
            {
                id = i.Id,
                title = i.Title,
                message = i.Message,
                category = i.LoaiThongBao,
                maHoSo = i.MaHoSo,
                hoTenHoSo = i.HoTenHoSo ?? "",
                tenVaccine = i.TenVaccine ?? "",
                tenMui = i.TenMui ?? "",
                soMui = i.SoMui,
                url = "/ThongBao/Index",
                createdAt = i.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
            }).ToList();

            return Ok(new { success = true, items = result });
        }

        [HttpPost("mark-desktop-pushed")]
        // đánh dấu đã đẩy desktop
        public IActionResult MarkDesktopPushed([FromBody] MarkDesktopPushedRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });

            if (request?.NotificationIds == null || request.NotificationIds.Count == 0)
                return Ok(new { success = true, marked = 0 });

            thongBaoDAL.MarkDesktopPushed(userId.Value, request.NotificationIds);

            return Ok(new { success = true, marked = request.NotificationIds.Count });
        }
    }

    public class MarkDesktopPushedRequest
    {
        public List<int> NotificationIds { get; set; } = new();
    }
}
