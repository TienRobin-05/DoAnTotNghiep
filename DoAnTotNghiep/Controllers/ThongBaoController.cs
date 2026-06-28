using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>Tiếp nhận các thao tác xem và cập nhật thông báo của người dùng.</summary>
    public class ThongBaoController : Controller
    {
        private readonly ThongBao_DAL thongBaoDAL;

        public ThongBaoController(ThongBao_DAL thongBaoDAL)
        {
            this.thongBaoDAL = thongBaoDAL;
        }

        // hiển thị danh sách thông báo
        public IActionResult Index(string? trangThai, string? tuKhoa)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            var trangThaiHopLe = trangThai is "chua-doc" or "da-doc" ? trangThai : "tat-ca";
            bool? daDoc = trangThaiHopLe switch
            {
                "chua-doc" => false,
                "da-doc" => true,
                _ => null
            };

            var soLuongTheoTrangThai = thongBaoDAL.DemTheoTrangThai(maTaiKhoan.Value, tuKhoa);
            var soLuongTheoNhom = thongBaoDAL.DemTheoNhom(maTaiKhoan.Value, daDoc, tuKhoa);

            // Lay toan bo danh sach thong bao (khong phan trang) de chia vao 3 cot
            const int tatCa = 9999;
            var model = new ThongBaoIndexViewModel
            {
                DanhSach = thongBaoDAL.LayTrangTheoTaiKhoan(maTaiKhoan.Value, daDoc, tuKhoa, 1, tatCa),
                TrangThai = trangThaiHopLe,
                TuKhoa = tuKhoa?.Trim() ?? string.Empty,
                TongThongBaoTaiKhoan = thongBaoDAL.DemTongThongBao(maTaiKhoan.Value),
                TongTatCa = soLuongTheoTrangThai.TatCa,
                TongChuaDoc = soLuongTheoTrangThai.ChuaDoc,
                TongDaDoc = soLuongTheoTrangThai.DaDoc,
                TongQuaHan = soLuongTheoNhom.QuaHan,
                TongDenLich = soLuongTheoNhom.DenLich,
                TongDaCapNhat = soLuongTheoNhom.DaCapNhat
            };

            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // đánh dấu tất cả đã đọc
        public IActionResult DanhDauTatCaDaDoc(string? trangThai, string? tuKhoa)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var soThongBaoDaCapNhat = thongBaoDAL.DanhDauTatCaDaDoc(maTaiKhoan.Value);
            TempData["ThongBao"] = soThongBaoDaCapNhat > 0
                ? $"Đã đánh dấu {soThongBaoDaCapNhat} thông báo là đã đọc"
                : "Không còn thông báo chưa đọc";

            return RedirectToAction(nameof(Index), new { trangThai, tuKhoa });
        }

        // xem chi tiết thông báo
        public IActionResult Details(int id = 0, int maThongBao = 0, bool chuyenHuongLichTiem = false)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            var maThongBaoCanXem = maThongBao > 0 ? maThongBao : id;
            var thongBao = thongBaoDAL.LayTheoId(maThongBaoCanXem, maTaiKhoan.Value);
            if (thongBao == null) return RedirectToAction(nameof(Index));

            if (!thongBao.DaDoc)
            {
                if (thongBaoDAL.DanhDauDaDoc(maThongBaoCanXem, maTaiKhoan.Value))
                {
                    TempData["ThongBao"] = "Đã đánh dấu thông báo là đã đọc";
                    thongBao.DaDoc = true;
                }
                else
                {
                    TempData["ThongBao"] = "Cập nhật trạng thái thông báo thất bại";
                    TempData["LoaiThongBao"] = "error";
                }
            }

            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            if (chuyenHuongLichTiem && thongBao.MaHoSo.HasValue)
            {
                return RedirectToAction("Index", "LichTiem", new { maHoSo = thongBao.MaHoSo.Value });
            }

            return View(thongBao);
        }

        // lấy mã tài khoản người dùng
        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return vaiTro == "User" ? maTaiKhoan.Value : null;
        }
    }
}
