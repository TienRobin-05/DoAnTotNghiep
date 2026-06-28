using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class LichSuTiemController : Controller
    {
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly LichSuTiem_DAL lichSuTiemDAL;

        public LichSuTiemController(HoSoSucKhoe_DAL hoSoSucKhoeDAL, LichSuTiem_DAL lichSuTiemDAL)
        {
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.lichSuTiemDAL = lichSuTiemDAL;
        }

        // hiển thị danh sách hồ sơ
        public IActionResult ChonHoSo(int? maHoSo)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var danhSachHoSo = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            var lichSuTheoHoSo = danhSachHoSo.ToDictionary(
                hoSo => hoSo.MaHoSo,
                hoSo => lichSuTiemDAL.LayDanhSachTheoHoSo(hoSo.MaHoSo, maTaiKhoan.Value));

            var maHoSoDuocChon = maHoSo.HasValue && lichSuTheoHoSo.ContainsKey(maHoSo.Value)
                ? maHoSo.Value
                : danhSachHoSo.FirstOrDefault()?.MaHoSo;

            ViewBag.LichSuTheoHoSo = lichSuTheoHoSo;
            ViewBag.MaHoSoDuocChon = maHoSoDuocChon;
            return View(danhSachHoSo);
        }

        // hiển thị lịch sử tiêm của hồ sơ
        public IActionResult Index(int maHoSo)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            // Kiểm tra hồ sơ thuộc đúng tài khoản đang đăng nhập trước khi xem lịch sử tiêm.
            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null) return NotFound();

            ViewBag.HoTenHoSo = hoSo.HoTen;
            return View(lichSuTiemDAL.LayDanhSachTheoHoSo(maHoSo, maTaiKhoan.Value));
        }

        // lấy mã tài khoản người dùng
        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "User") return null;

            return maTaiKhoan.Value;
        }
    }
}
