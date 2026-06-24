using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class NotificationTestController : Controller
    {
        private readonly ThongBao_DAL thongBaoDAL;
        private readonly PushNotificationService pushService;
        private readonly PushSubscription_DAL pushSubDAL;
        private readonly LichTiem_DAL lichTiemDAL;

        public NotificationTestController(
            ThongBao_DAL thongBaoDAL,
            PushNotificationService pushService,
            PushSubscription_DAL pushSubDAL,
            LichTiem_DAL lichTiemDAL)
        {
            this.thongBaoDAL = thongBaoDAL;
            this.pushService = pushService;
            this.pushSubDAL = pushSubDAL;
            this.lichTiemDAL = lichTiemDAL;
        }

        [HttpPost]
        public IActionResult TestPush()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
                return Json(new { success = false, reason = "NOT_LOGGED_IN", message = "Vui lòng đăng nhập." });

            var subs = pushSubDAL.LayTheoTaiKhoan(maTaiKhoan.Value);
            if (subs.Count == 0)
                return Json(new { success = false, reason = "NO_PUSH_SUBSCRIPTION", message = "Bạn chưa bật thông báo đẩy. Hãy bật thông báo ở nút phía trên." });

            var count = 0;
            foreach (var sub in subs)
            {
                pushService.GuiThongBao(maTaiKhoan.Value, 0, "Pharmacy City - Test", "Đây là thông báo đẩy thử nghiệm. Nếu bạn thấy dòng này, Web Push đã hoạt động trên laptop của bạn.");
                count++;
            }

            return Json(new { success = true, message = $"Đã gửi test push đến {count} thiết bị.", subscriptionCount = count });
        }

        [HttpPost]
        public IActionResult TestUpcomingReminder()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
                return Json(new { success = false, reason = "NOT_LOGGED_IN", message = "Vui lòng đăng nhập." });

            var tao = thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            if (tao > 0)
            {
                var subs = pushSubDAL.LayTheoTaiKhoan(maTaiKhoan.Value);
                return Json(new { success = true, message = $"Đã tạo {tao} thông báo đến lịch. Số thiết bị push: {subs.Count}.", notificationsCreated = tao, subscriptionCount = subs.Count });
            }

            return Json(new { success = false, reason = "NO_UPCOMING_SCHEDULE", message = "Không có lịch tiêm nào trong 3 ngày tới. Hãy kiểm tra dữ liệu lịch tiêm của bạn." });
        }
    }
}
