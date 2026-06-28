using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class LichTiemController : Controller
    {
        private readonly LichTiem_DAL lichTiemDAL;
        private readonly LichSuTiem_DAL lichSuTiemDAL;
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly ThongBao_DAL thongBaoDAL;
        private readonly TaoLichTiemService taoLichTiemService;

        public LichTiemController(
            LichTiem_DAL lichTiemDAL,
            LichSuTiem_DAL lichSuTiemDAL,
            HoSoSucKhoe_DAL hoSoSucKhoeDAL,
            ThongBao_DAL thongBaoDAL,
            TaoLichTiemService taoLichTiemService)
        {
            this.lichTiemDAL = lichTiemDAL;
            this.lichSuTiemDAL = lichSuTiemDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.thongBaoDAL = thongBaoDAL;
            this.taoLichTiemService = taoLichTiemService;
        }

        // hiển thị danh sách hồ sơ để chọn
        public IActionResult ChonHoSo()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            var danhSachHoSo = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            return View(danhSachHoSo);
        }

        // hiển thị lịch tiêm của hồ sơ
        public IActionResult Index(int maHoSo, string? hienThi)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hồ sơ sức khỏe";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(ChonHoSo));
            }

            var ketQuaTaoLich = taoLichTiemService.TaoLichTiemChoHoSo(maHoSo);
            if (ketQuaTaoLich.SoMuiTiemVaccine == 0)
            {
                TempData["ThongBao"] = "Chưa có dữ liệu mũi tiêm vaccine.";
                TempData["LoaiThongBao"] = "info";
            }

            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            ViewBag.HoTenHoSo = hoSo.HoTen;
            ViewBag.CanhBaoDoiNgaySinh = hoSoSucKhoeDAL.KiemTraHoSoCoCanhBaoDoiNgaySinh(maHoSo, maTaiKhoan.Value);
            ViewBag.MaHoSoHienTai = maHoSo;
            ViewBag.NgaySinhText = hoSo.NgaySinh.ToString("dd/MM/yyyy");
            ViewBag.NgaySinhIso = hoSo.NgaySinh.ToString("yyyy-MM-dd");

            var tatCaLichTiem = lichTiemDAL.LayDanhSachTheoHoSo(maHoSo, maTaiKhoan.Value);

            // Lấy ngày tiêm thực tế cho tất cả lịch tiêm của hồ sơ (1 query)
            var ngayTiemThucTeMap = lichSuTiemDAL.LayNgayTiemThucTeTheoHoSo(maHoSo);

            // Serialize to JSON-safe objects for client-side rendering
            var scheduleData = tatCaLichTiem.Select(l =>
            {
                var injectedDate = ngayTiemThucTeMap.TryGetValue(l.MaLichTiem, out var ngayTiem) ? ngayTiem : (DateTime?)null;
                return new
                {
                    id = l.MaLichTiem,
                    vaccineName = l.TenVaccine ?? "",
                    doseName = $"Mũi {l.SoMui}",
                    groupName = l.NhomVaccine ?? "",
                    expectedDate = l.NgayTiemDuKien.ToString("yyyy-MM-dd"),
                    injectedDate = injectedDate?.ToString("yyyy-MM-dd"),
                    isDone = LaDaTiem(l),
                    note = l.GhiChu ?? ""
                };
            }).ToList();

            ViewBag.ScheduleDataJson = System.Text.Json.JsonSerializer.Serialize(scheduleData);

            var dsNhom = tatCaLichTiem.Select(l => l.NhomVaccine)
                .Where(n => !string.IsNullOrEmpty(n)).Distinct().OrderBy(n => n).ToList();
            ViewBag.DanhSachNhomVaccine = dsNhom;
            ViewBag.MaHoSo = maHoSo;
            ViewBag.HienThi = hienThi ?? "time";

            return View(tatCaLichTiem);
        }

        // kiểm tra trạng thái mũi tiêm
        private static bool LaDenLich(LichTiem lich)
        {
            return !LaDaTiem(lich) && lich.NgayTiemDuKien.Date == DateTime.Today;
        }

        [HttpGet]
        public IActionResult CapNhatDaTiem(int maLichTiem)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var lichTiem = lichTiemDAL.LayChiTietCoKiemTraChuSoHuu(maLichTiem, maTaiKhoan.Value);
            if (lichTiem == null) return NotFound();

            // Pre-fill với ngày dự kiến (nếu chưa từng cập nhật) hoặc ngày tiêm thực tế cũ
            var lichSuCu = lichSuTiemDAL.LayTheoMaLichTiem(maLichTiem);
            ViewBag.NgayTiemThucTe = lichSuCu != null
                ? lichSuCu.NgayTiemThucTe.ToString("yyyy-MM-dd")
                : lichTiem.NgayTiemDuKien.ToString("yyyy-MM-dd");
            ViewBag.GhiChu = lichSuCu?.GhiChu ?? string.Empty;
            return View(lichTiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // xử lý cập nhật mũi tiêm
        public IActionResult CapNhatDaTiem(int maLichTiem, DateTime ngayTiemThucTe, string ghiChu)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var lichTiem = lichTiemDAL.LayChiTietCoKiemTraChuSoHuu(maLichTiem, maTaiKhoan.Value);
            if (lichTiem == null) return NotFound();

            // Validation
            var loi = ValidateNgayTiemThucTe(lichTiem, ngayTiemThucTe);
            if (loi != null)
            {
                ViewBag.ThongBao = loi;
                ViewBag.NgayTiemThucTe = ngayTiemThucTe.ToString("yyyy-MM-dd");
                ViewBag.GhiChu = ghiChu;
                return View(lichTiem);
            }

            var lichSu = new LichSuTiem
            {
                MaLichTiem = maLichTiem,
                NgayTiemThucTe = ngayTiemThucTe,
                GhiChu = ghiChu ?? string.Empty,
                NgayCapNhat = DateTime.Now
            };

            if (lichSuTiemDAL.KiemTraDaCoLichSu(maLichTiem))
            {
                // Update record cũ
                if (!lichSuTiemDAL.CapNhat(lichSu))
                {
                    ViewBag.ThongBao = "Cập nhật lịch sử tiêm thất bại.";
                    return View(lichTiem);
                }
            }
            else
            {
                // Tạo mới
                if (!lichTiemDAL.CapNhatDaTiemVaGhiLichSu(lichSu, maTaiKhoan.Value))
                {
                    TempData["ThongBao"] = "Cập nhật lịch tiêm thất bại.";
                    TempData["LoaiThongBao"] = "warning";
                    return RedirectToAction(nameof(Index), new { maHoSo = lichTiem.MaHoSo });
                }
            }

            // Tính lại các mũi tiếp theo
            var soLichDaDieuChinh = taoLichTiemService.DieuChinhLichMuiTiepTheoSauKhiTiem(
                lichTiem.MaHoSo,
                lichTiem.MaMuiTiem,
                ngayTiemThucTe);

            TempData["ThongBao"] = soLichDaDieuChinh > 0
                ? $"Cập nhật lịch tiêm thành công. Hệ thống đã điều chỉnh {soLichDaDieuChinh} mũi tiếp theo."
                : "Cập nhật lịch tiêm thành công";
            return RedirectToAction(nameof(Index), new { maHoSo = lichTiem.MaHoSo });
        }

        private string? ValidateNgayTiemThucTe(LichTiem lichTiem, DateTime ngayTiemThucTe)
        {
            if (ngayTiemThucTe.Date < lichTiem.NgaySinhHoSo.Date)
                return "Ngày tiêm thực tế không được nhỏ hơn ngày sinh.";

            if (ngayTiemThucTe.Date > DateTime.Today)
                return "Ngày tiêm thực tế không được lớn hơn ngày hiện tại.";

            var ngayMuiTruoc = lichSuTiemDAL.LayNgayTiemThucTeMuiTruoc(lichTiem.MaHoSo, lichTiem.MaMuiTiem);
            if (ngayMuiTruoc.HasValue && ngayTiemThucTe.Date < ngayMuiTruoc.Value.Date)
                return "Ngày tiêm thực tế không được nhỏ hơn ngày tiêm thực tế của mũi trước.";

            var ngayMuiSau = lichSuTiemDAL.LayNgayTiemThucTeMuiSau(lichTiem.MaHoSo, lichTiem.MaMuiTiem);
            if (ngayMuiSau.HasValue && ngayTiemThucTe.Date > ngayMuiSau.Value.Date)
                return "Ngày tiêm thực tế không được lớn hơn ngày tiêm thực tế của mũi sau.";

            return null;
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

        private static bool LaLichTiemNenHienThi(LichTiem lich)
        {
            if (!LaVaccineNhacHangNam(lich))
            {
                return true;
            }

            return lich.TrangThai == "Đã tiêm" || lich.NgayTiemDuKien.Year == DateTime.Today.Year;
        }

        // lọc lịch tiêm theo trạng thái
        private static List<LichTiem> LocLichTiemTheoTrangThai(List<LichTiem> danhSach, string boLoc)
        {
            return boLoc switch
            {
                "da-tiem" => danhSach.Where(LaDaTiem).ToList(),
                "qua-han" => danhSach.Where(LaQuaHan).ToList(),
                "sap-toi" => danhSach.Where(LaSapToi).ToList(),
                _ => danhSach
            };
        }

        // chuẩn hóa/xử lý chuỗi
        private static string ChuanHoaBoLoc(string? boLoc)
        {
            return boLoc switch
            {
                "da-tiem" => "da-tiem",
                "qua-han" => "qua-han",
                "sap-toi" => "sap-toi",
                _ => "tat-ca"
            };
        }

        // kiểm tra trạng thái mũi tiêm
        private static bool LaDaTiem(LichTiem lich)
        {
            return string.Equals(lich.TrangThai, "Đã tiêm", StringComparison.OrdinalIgnoreCase);
        }

        // kiểm tra trạng thái mũi tiêm
        private static bool LaQuaHan(LichTiem lich)
        {
            return !LaDaTiem(lich) && lich.NgayTiemDuKien.Date < DateTime.Today;
        }

        // kiểm tra trạng thái mũi tiêm
        private static bool LaSapToi(LichTiem lich)
        {
            return !LaDaTiem(lich) && lich.NgayTiemDuKien.Date >= DateTime.Today;
        }

        // kiểm tra vaccine nhắc năm
        private static bool LaVaccineNhacHangNam(LichTiem lich)
        {
            var noiDung = BoDauTiengViet($"{lich.TenVaccine} {lich.NhomVaccine}").ToLowerInvariant();
            return noiDung.Contains("cum")
                || noiDung.Contains("hang nam");
        }

        // chuẩn hóa/xử lý chuỗi
        private static string BoDauTiengViet(string giaTri)
        {
            var normalized = giaTri.Normalize(System.Text.NormalizationForm.FormD);
            var builder = new System.Text.StringBuilder();
            foreach (var kyTu in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(kyTu) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(kyTu);
                }
            }

            return builder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}
