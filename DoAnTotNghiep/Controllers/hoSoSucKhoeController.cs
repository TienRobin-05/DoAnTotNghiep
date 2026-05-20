using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class HoSoSucKhoeController : Controller
    {
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly TaiKhoan_DAL taiKhoanDAL;

        public HoSoSucKhoeController(HoSoSucKhoe_DAL hoSoSucKhoeDAL, TaiKhoan_DAL taiKhoanDAL)
        {
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.taiKhoanDAL = taiKhoanDAL;
        }

        [HttpGet]
        public IActionResult CapNhatThongTinCaNhan()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!string.Equals(HttpContext.Session.GetString("VaiTro"), "User", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (hoSoSucKhoeDAL.KiemTraTaiKhoanDaCoHoSo(maTaiKhoan.Value))
            {
                return RedirectToAction("Index", "NguoiDung");
            }

            GanThongTinTaiKhoanLenView(maTaiKhoan.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhatThongTinCaNhan(
            string hoTen,
            DateTime? ngaySinh,
            string gioiTinh,
            double? chieuCao,
            double? canNang,
            string tienSuBenh,
            string diUng)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!string.Equals(HttpContext.Session.GetString("VaiTro"), "User", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (hoSoSucKhoeDAL.KiemTraTaiKhoanDaCoHoSo(maTaiKhoan.Value))
            {
                return RedirectToAction("Index", "NguoiDung");
            }

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                return LoiCapNhat(maTaiKhoan.Value, "Họ tên không được bỏ trống");
            }
            if (ngaySinh == null)
            {
                return LoiCapNhat(maTaiKhoan.Value, "Ngày sinh không được bỏ trống");
            }
            if (ngaySinh.Value.Date > DateTime.Today)
            {
                return LoiCapNhat(maTaiKhoan.Value, "Ngày sinh không được lớn hơn ngày hiện tại");
            }
            if (chieuCao.HasValue && chieuCao.Value <= 0)
            {
                return LoiCapNhat(maTaiKhoan.Value, "Chiều cao phải lớn hơn 0");
            }
            if (canNang.HasValue && canNang.Value <= 0)
            {
                return LoiCapNhat(maTaiKhoan.Value, "Cân nặng phải lớn hơn 0");
            }

            var hoSo = new HoSoSucKhoe
            {
                MaTaiKhoan = maTaiKhoan.Value,
                HoTen = hoTen,
                NgaySinh = ngaySinh.Value,
                GioiTinh = gioiTinh ?? string.Empty,
                ChieuCao = chieuCao,
                CanNang = canNang,
                TienSuBenh = tienSuBenh ?? string.Empty,
                DiUng = diUng ?? string.Empty,
                NgayTao = DateTime.Now
            };

            if (hoSoSucKhoeDAL.Them(hoSo))
            {
                taiKhoanDAL.CapNhatHoTen(maTaiKhoan.Value, hoTen);
                HttpContext.Session.SetString("HoTen", hoTen);
                return RedirectToAction("Index", "NguoiDung");
            }

            return LoiCapNhat(maTaiKhoan.Value, "Lưu thông tin cá nhân thất bại, vui lòng thử lại");
        }

        private IActionResult LoiCapNhat(int maTaiKhoan, string thongBao)
        {
            GanThongTinTaiKhoanLenView(maTaiKhoan);
            ViewBag.ThongBao = thongBao;
            return View();
        }

        private void GanThongTinTaiKhoanLenView(int maTaiKhoan)
        {
            var soDienThoai = HttpContext.Session.GetString("SoDienThoai");
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                var taiKhoan = taiKhoanDAL.LayTaiKhoanTheoId(maTaiKhoan);
                soDienThoai = taiKhoan?.SoDienThoai ?? string.Empty;
                email = taiKhoan?.Email ?? string.Empty;
            }

            ViewBag.SoDienThoai = soDienThoai;
            ViewBag.Email = email;
        }
    }
}
