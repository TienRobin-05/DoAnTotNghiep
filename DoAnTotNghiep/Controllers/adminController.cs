using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class adminController : Controller
    {
        private readonly taiKhoanDAL taiKhoanDAL;
        private readonly vaccineDAL vaccineDAL;
        private readonly lichTiemDAL lichTiemDAL;
        private readonly cauHoiTuVanDAL cauHoiTuVanDAL;

        public adminController(taiKhoanDAL taiKhoanDAL, vaccineDAL vaccineDAL, lichTiemDAL lichTiemDAL, cauHoiTuVanDAL cauHoiTuVanDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
            this.vaccineDAL = vaccineDAL;
            this.lichTiemDAL = lichTiemDAL;
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
        }

        public IActionResult index()
        {
            if (!laAdmin())
            {
                return RedirectToAction("dangNhap", "taiKhoan");
            }

            ViewBag.SoTaiKhoan = taiKhoanDAL.layTatCa().Count;
            ViewBag.SoVaccine = vaccineDAL.layTatCa().Count;
            ViewBag.SoLichTiem = lichTiemDAL.layTatCa().Count;
            ViewBag.SoCauHoiChoTraLoi = cauHoiTuVanDAL.layTatCa().Count(x => x.trangThai != "Đã trả lời");
            return View();
        }

        public IActionResult taiKhoan()
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            return View(taiKhoanDAL.layTatCa());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult doiTrangThaiTaiKhoan(int maTaiKhoan, bool trangThai)
        {
            if (!laAdmin()) return RedirectToAction("dangNhap", "taiKhoan");
            taiKhoanDAL.doiTrangThai(maTaiKhoan, trangThai);
            return RedirectToAction(nameof(taiKhoan));
        }

        private bool laAdmin()
        {
            return string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
