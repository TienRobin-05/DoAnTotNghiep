using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp LichTiemController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
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

        // Mục đích: action ChonHoSo xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult ChonHoSo()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            var danhSachHoSo = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            return View(danhSachHoSo);
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index(int maHoSo, string? trangThai)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            // Chỉ cho user xem lịch tiêm của hồ sơ thuộc tài khoản đang đăng nhập.
            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null)
            {
                TempData["ThongBao"] = "Không tìm thấy hồ sơ sức khỏe";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(ChonHoSo));
            }

            // Mỗi lần xem lịch, hệ thống tính tuổi và chỉ tạo thêm các mũi phù hợp còn thiếu, không tạo trùng.
            var ketQuaTaoLich = taoLichTiemService.TaoLichTiemChoHoSo(maHoSo);
            if (ketQuaTaoLich.SoMuiTiemVaccine == 0)
            {
                TempData["ThongBao"] = "Chưa có dữ liệu mũi tiêm vaccine. Vui lòng thêm mũi tiêm ở trang quản trị.";
                TempData["LoaiThongBao"] = "info";
            }
            else if (ketQuaTaoLich.SoLichTiemDaTao > 0)
            {
                TempData["ThongBao"] = "Hệ thống đã tự động tạo lịch tiêm cho hồ sơ này";
                TempData["LoaiThongBao"] = "success";
            }

            // Sau khi tạo/kiểm tra lịch, tạo thông báo cho các lịch đã đến hạn hoặc quá hạn.
            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            ViewBag.HoTenHoSo = hoSo.HoTen;
            var danhSachLichTiem = lichTiemDAL.LayDanhSachTheoHoSo(maHoSo, maTaiKhoan.Value)
                .Where(LaLichTiemNenHienThi)
                .ToList();

            var boLoc = ChuanHoaBoLoc(trangThai);
            ViewBag.MaHoSo = maHoSo;
            ViewBag.BoLocTrangThai = boLoc;
            ViewBag.TongTatCa = danhSachLichTiem.Count;
            ViewBag.TongDaTiem = danhSachLichTiem.Count(LaDaTiem);
            ViewBag.TongQuaHan = danhSachLichTiem.Count(LaQuaHan);
            ViewBag.TongSapToi = danhSachLichTiem.Count(LaSapToi);
            danhSachLichTiem = LocLichTiemTheoTrangThai(danhSachLichTiem, boLoc);

            return View(danhSachLichTiem);
        }

        [HttpGet]
        // Mục đích: action CapNhatDaTiem xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult CapNhatDaTiem(int maLichTiem)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var lichTiem = lichTiemDAL.LayChiTietCoKiemTraChuSoHuu(maLichTiem, maTaiKhoan.Value);
            if (lichTiem == null) return NotFound();

            if (lichSuTiemDAL.KiemTraDaCoLichSu(maLichTiem))
            {
                TempData["ThongBao"] = "Lịch tiêm này đã được cập nhật lịch sử tiêm.";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(Index), new { maHoSo = lichTiem.MaHoSo });
            }

            ViewBag.NgayTiemThucTe = DateTime.Today.ToString("yyyy-MM-dd");
            return View(lichTiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action CapNhatDaTiem xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult CapNhatDaTiem(int maLichTiem, DateTime ngayTiemThucTe, string ghiChu)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var lichTiem = lichTiemDAL.LayChiTietCoKiemTraChuSoHuu(maLichTiem, maTaiKhoan.Value);
            if (lichTiem == null) return NotFound();

            // Ngày tiêm thực tế phải nằm trong khoảng từ ngày sinh đến ngày hiện tại.
            if (ngayTiemThucTe.Date < lichTiem.NgaySinhHoSo.Date)
            {
                ViewBag.ThongBao = "Ngày tiêm thực tế không được nhỏ hơn ngày sinh của hồ sơ.";
                ViewBag.NgayTiemThucTe = ngayTiemThucTe.ToString("yyyy-MM-dd");
                ViewBag.GhiChu = ghiChu;
                return View(lichTiem);
            }

            if (ngayTiemThucTe.Date > DateTime.Today)
            {
                ViewBag.ThongBao = "Ngày tiêm thực tế không được lớn hơn ngày hiện tại.";
                ViewBag.NgayTiemThucTe = ngayTiemThucTe.ToString("yyyy-MM-dd");
                ViewBag.GhiChu = ghiChu;
                return View(lichTiem);
            }

            // Không tạo lại lịch sử nếu lịch tiêm này đã được ghi nhận trước đó.
            if (lichSuTiemDAL.KiemTraDaCoLichSu(maLichTiem))
            {
                TempData["ThongBao"] = "Lịch tiêm này đã được cập nhật lịch sử tiêm.";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(Index), new { maHoSo = lichTiem.MaHoSo });
            }

            var daCapNhat = lichTiemDAL.CapNhatDaTiemVaGhiLichSu(new LichSuTiem
            {
                MaLichTiem = maLichTiem,
                NgayTiemThucTe = ngayTiemThucTe,
                GhiChu = ghiChu ?? string.Empty,
                NgayCapNhat = DateTime.Now
            }, maTaiKhoan.Value);

            if (!daCapNhat)
            {
                TempData["ThongBao"] = "Lịch tiêm này đã được cập nhật lịch sử tiêm.";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(Index), new { maHoSo = lichTiem.MaHoSo });
            }

            var soLichDaDieuChinh = taoLichTiemService.DieuChinhLichMuiTiepTheoSauKhiTiem(
                lichTiem.MaHoSo,
                lichTiem.MaMuiTiem,
                ngayTiemThucTe);

            TempData["ThongBao"] = soLichDaDieuChinh > 0
                ? $"Cập nhật lịch tiêm thành công. Hệ thống đã điều chỉnh {soLichDaDieuChinh} mũi tiếp theo theo ngày tiêm thực tế."
                : "Cập nhật lịch tiêm thành công";
            return RedirectToAction(nameof(Index), new { maHoSo = lichTiem.MaHoSo });
        }

        // Mục đích: action LayMaTaiKhoanUser xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
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

        private static bool LaDaTiem(LichTiem lich)
        {
            return string.Equals(lich.TrangThai, "Đã tiêm", StringComparison.OrdinalIgnoreCase);
        }

        private static bool LaQuaHan(LichTiem lich)
        {
            return !LaDaTiem(lich) && lich.NgayTiemDuKien.Date < DateTime.Today;
        }

        private static bool LaSapToi(LichTiem lich)
        {
            return !LaDaTiem(lich) && lich.NgayTiemDuKien.Date >= DateTime.Today;
        }

        private static bool LaVaccineNhacHangNam(LichTiem lich)
        {
            var noiDung = BoDauTiengViet($"{lich.TenVaccine} {lich.NhomVaccine}").ToLowerInvariant();
            return noiDung.Contains("cum")
                || noiDung.Contains("hang nam");
        }

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
