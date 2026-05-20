using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class lichTiemController : Controller
    {
        private readonly lichTiemDAL lichTiemDAL;
        private readonly lichSuTiemDAL lichSuTiemDAL;

        public lichTiemController(lichTiemDAL lichTiemDAL, lichSuTiemDAL lichSuTiemDAL)
        {
            this.lichTiemDAL = lichTiemDAL;
            this.lichSuTiemDAL = lichSuTiemDAL;
        }

        public IActionResult index()
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            return View(lichTiemDAL.layTheoTaiKhoan(maTaiKhoan.Value));
        }

        public IActionResult lichSu()
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            return View(lichSuTiemDAL.layTheoTaiKhoan(maTaiKhoan.Value));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult capNhatDaTiem(int maLichTiem, DateTime ngayTiemThucTe, string? ghiChu)
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            if (!lichTiemDAL.lichThuocTaiKhoan(maLichTiem, maTaiKhoan.Value)) return Forbid();

            lichSuTiemDAL.them(new lichSuTiemModels
            {
                maLichTiem = maLichTiem,
                ngayTiemThucTe = ngayTiemThucTe,
                ghiChu = ghiChu
            });
            lichTiemDAL.capNhatTrangThai(maLichTiem, "Đã tiêm");
            return RedirectToAction(nameof(index));
        }

        private int? layMaTaiKhoan()
        {
            return HttpContext.Session.GetInt32("MaTaiKhoan");
        }
    }
}
