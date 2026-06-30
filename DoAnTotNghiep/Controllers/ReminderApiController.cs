using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/reminders")]
    [ApiController]
    public class ReminderApiController : ControllerBase
    {
        private readonly TaiKhoan_DAL taiKhoanDAL;

        public ReminderApiController(TaiKhoan_DAL taiKhoanDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
        }

        private int? GetCurrentUserId()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return vaiTro == "User" ? maTaiKhoan.Value : null;
        }

        [HttpGet("push-enabled")]
        // kiểm tra trạng thái thông báo đẩy
        public IActionResult GetPushEnabled()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { enabled = false });

            var enabled = taiKhoanDAL.GetPushNotificationEnabled(userId.Value);
            return Ok(new { enabled });
        }

        [HttpPost("push-enabled")]
        // bật/tắt thông báo đẩy
        public IActionResult SetPushEnabled([FromBody] PushEnabledRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });

            taiKhoanDAL.SetPushNotificationEnabled(userId.Value, request.Enabled);

            return Ok(new { success = true, enabled = request.Enabled });
        }
    }

    public class PushEnabledRequest
    {
        public bool Enabled { get; set; }
    }
}
