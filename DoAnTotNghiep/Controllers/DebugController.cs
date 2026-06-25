using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class DebugController : Controller
    {
        private readonly TaoLichTiemService taoLichTiemService;
        private readonly ThongBao_DAL thongBaoDAL;
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly PushSubscription_DAL pushSubDAL;
        private readonly PushNotificationService pushService;

        public DebugController(
            TaoLichTiemService taoLichTiemService,
            ThongBao_DAL thongBaoDAL,
            HoSoSucKhoe_DAL hoSoSucKhoeDAL,
            PushSubscription_DAL pushSubDAL,
            PushNotificationService pushService)
        {
            this.taoLichTiemService = taoLichTiemService;
            this.thongBaoDAL = thongBaoDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.pushSubDAL = pushSubDAL;
            this.pushService = pushService;
        }

        private int? LayMaTaiKhoan()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
                return null;
            return maTaiKhoan.Value;
        }

        [HttpGet]
        public IActionResult GetSchedules(int maHoSo)
        {
            var maTaiKhoan = LayMaTaiKhoan();
            if (maTaiKhoan == null)
                return Json(new { error = "Chua dang nhap" });

            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null)
                return Json(new { error = "Khong tim thay ho so hoac khong thuoc tai khoan" });

            var schedules = taoLichTiemService.LayDanhSachLichTheoHoSo(maHoSo);

            return Json(new
            {
                profileId = maHoSo,
                profileName = hoSo.HoTen,
                birthDate = hoSo.NgaySinh.ToString("yyyy-MM-dd"),
                scheduleCount = schedules.Count,
                schedules = schedules.Select(s => new
                {
                    id = s.MaLichTiem,
                    vaccineName = s.TenVaccine,
                    doseName = s.TenMui,
                    doseNumber = s.SoMui,
                    vaccinationDate = s.NgayTiemDuKien.ToString("yyyy-MM-dd"),
                    daysFromNow = (s.NgayTiemDuKien.Date - DateTime.Today).Days,
                    status = s.TrangThai,
                    note = s.GhiChu
                })
            });
        }

        [HttpPost]
        public IActionResult CreateDemoUpcomingSchedule(int maHoSo)
        {
            var maTaiKhoan = LayMaTaiKhoan();
            if (maTaiKhoan == null)
                return Json(new { error = "Chua dang nhap" });

            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null)
                return Json(new { error = "Khong tim thay ho so" });

            // Tạo lịch demo
            var demoTao = taoLichTiemService.TaoLichTiemDemoSapToi(maHoSo);
            var taoTB = thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);

            // Lấy lại danh sách để kiểm tra
            var schedules = taoLichTiemService.LayDanhSachLichTheoHoSo(maHoSo);
            var upcomingSchedule = schedules.FirstOrDefault(s => s.NgayTiemDuKien.Date >= DateTime.Today);

            return Json(new
            {
                success = true,
                message = $"Da tao lich demo: {demoTao}, da tao thong bao: {taoTB}",
                schedule = upcomingSchedule == null ? null : new
                {
                    id = upcomingSchedule.MaLichTiem,
                    vaccineName = upcomingSchedule.TenVaccine,
                    doseName = upcomingSchedule.TenMui,
                    vaccinationDate = upcomingSchedule.NgayTiemDuKien.ToString("yyyy-MM-dd"),
                    daysFromNow = (upcomingSchedule.NgayTiemDuKien.Date - DateTime.Today).Days,
                    status = upcomingSchedule.TrangThai
                },
                notificationCreated = taoTB
            });
        }

        [HttpPost]
        public IActionResult TestPush()
        {
            var maTaiKhoan = LayMaTaiKhoan();
            if (maTaiKhoan == null)
                return Json(new { success = false, message = "Vui long dang nhap." });

            var subs = pushSubDAL.LayTheoTaiKhoan(maTaiKhoan.Value);
            if (subs.Count == 0)
                return Json(new { success = false, message = "Chua bat thong bao day. Hay bat thong bao o nut phia tren." });

            var count = 0;
            foreach (var sub in subs)
            {
                pushService.GuiThongBao(maTaiKhoan.Value, 0, "Pharmacy City - Test",
                    "Thong bao day test tu Debug API. Web Push dang hoat dong tren laptop cua ban.");
                count++;
            }

            return Json(new { success = true, message = $"Da gui test push den {count} thiet bi.", subscriptionCount = count });
        }
    }
}
