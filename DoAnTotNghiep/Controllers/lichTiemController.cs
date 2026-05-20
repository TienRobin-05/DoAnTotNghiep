using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class LichTiemController : Controller
    {
        private readonly LichTiem_DAL lichTiemDAL;
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly MuiTiemVaccine_DAL muiTiemVaccineDAL;

        public LichTiemController(
            LichTiem_DAL lichTiemDAL,
            HoSoSucKhoe_DAL hoSoSucKhoeDAL,
            MuiTiemVaccine_DAL muiTiemVaccineDAL)
        {
            this.lichTiemDAL = lichTiemDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.muiTiemVaccineDAL = muiTiemVaccineDAL;
        }

        public IActionResult ChonHoSo()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var danhSachHoSo = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            return View(danhSachHoSo);
        }

        public IActionResult Index(int maHoSo)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null) return NotFound();

            if (!lichTiemDAL.KiemTraHoSoCoLichTiem(maHoSo))
            {
                TaoLichTiemTuDong(hoSo);
            }

            ViewBag.HoTenHoSo = hoSo.HoTen;
            return View(lichTiemDAL.LayDanhSachTheoHoSo(maHoSo, maTaiKhoan.Value));
        }

        private void TaoLichTiemTuDong(HoSoSucKhoe hoSo)
        {
            var danhSachMuiTiem = muiTiemVaccineDAL.LayDanhSach();

            foreach (var muiTiem in danhSachMuiTiem)
            {
                if (lichTiemDAL.KiemTraLichTonTai(hoSo.MaHoSo, muiTiem.MaMuiTiem))
                {
                    continue;
                }

                var soTuoi = muiTiem.DoTuoiKhuyenNghi ?? muiTiem.DoTuoiToiThieu ?? 0;
                var donViTuoi = string.IsNullOrWhiteSpace(muiTiem.DonViTuoi) ? "ngày" : muiTiem.DonViTuoi;
                var ngayTiemDuKien = TinhNgayTiemDuKien(hoSo.NgaySinh, soTuoi, donViTuoi);

                lichTiemDAL.Them(new LichTiem
                {
                    MaHoSo = hoSo.MaHoSo,
                    MaMuiTiem = muiTiem.MaMuiTiem,
                    NgayTiemDuKien = ngayTiemDuKien,
                    TrangThai = "Chưa tiêm",
                    GhiChu = "Tự động tạo từ phác đồ mũi tiêm vaccine"
                });
            }
        }

        private static DateTime TinhNgayTiemDuKien(DateTime ngaySinh, int soTuoi, string donViTuoi)
        {
            return donViTuoi.Trim().ToLower() switch
            {
                "năm" => ngaySinh.AddYears(soTuoi),
                "tháng" => ngaySinh.AddMonths(soTuoi),
                _ => ngaySinh.AddDays(soTuoi)
            };
        }

        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase)) return null;

            return maTaiKhoan.Value;
        }
    }
}
