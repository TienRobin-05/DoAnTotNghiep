using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp AdminController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class AdminController : Controller
    {
        private readonly TaiKhoan_DAL taiKhoanDAL;
        private readonly Vaccine_DAL vaccineDAL;
        private readonly MuiTiemVaccine_DAL muiTiemVaccineDAL;
        private readonly LichTiem_DAL lichTiemDAL;
        private readonly CauHoiTuVan_DAL cauHoiTuVanDAL;
        private readonly BaiVietCamNang_DAL baiVietDAL;

        public AdminController(
            TaiKhoan_DAL taiKhoanDAL,
            Vaccine_DAL vaccineDAL,
            MuiTiemVaccine_DAL muiTiemVaccineDAL,
            LichTiem_DAL lichTiemDAL,
            CauHoiTuVan_DAL cauHoiTuVanDAL,
            BaiVietCamNang_DAL baiVietDAL)
        {
            this.taiKhoanDAL = taiKhoanDAL;
            this.vaccineDAL = vaccineDAL;
            this.muiTiemVaccineDAL = muiTiemVaccineDAL;
            this.lichTiemDAL = lichTiemDAL;
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
            this.baiVietDAL = baiVietDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var danhSachVaccine = vaccineDAL.LayDanhSach();
            var danhSachMuiTiem = muiTiemVaccineDAL.LayDanhSach();
            var danhSachCauHoi = cauHoiTuVanDAL.LayTatCa();
            var danhSachBaiViet = baiVietDAL.LayTatCaChoAdmin();

            ViewBag.HoTen = HttpContext.Session.GetString("HoTen");
            ViewBag.SoTaiKhoan = taiKhoanDAL.DemTatCa();
            ViewBag.SoVaccine = danhSachVaccine.Count;
            ViewBag.SoMuiTiem = danhSachMuiTiem.Count;
            ViewBag.SoLichTiem = lichTiemDAL.DemTatCa();
            ViewBag.SoBaiViet = danhSachBaiViet.Count;
            ViewBag.SoCauHoiChoTraLoi = danhSachCauHoi.Count(x => x.TrangThai != "Đã trả lời");
            ViewBag.DanhSachVaccine = danhSachVaccine.Take(5).ToList();
            ViewBag.CauHoiMoi = danhSachCauHoi.Take(5).ToList();
            ViewBag.BaiVietMoi = danhSachBaiViet.Take(4).ToList();
            return View();
        }

        // Mục đích: action taiKhoan xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult taiKhoan()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            return View(taiKhoanDAL.LayTatCa());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action doiTrangThaiTaiKhoan xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult doiTrangThaiTaiKhoan(int maTaiKhoan, bool trangThai)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var taiKhoan = taiKhoanDAL.LayTaiKhoanTheoId(maTaiKhoan);
            if (taiKhoan == null)
            {
                TempData["ThongBao"] = "Không tìm thấy tài khoản cần cập nhật";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(taiKhoan));
            }

            var maTaiKhoanDangNhap = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoanDangNhap == maTaiKhoan && !trangThai)
            {
                TempData["ThongBao"] = "Không thể khóa tài khoản đang đăng nhập.";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(taiKhoan));
            }

            taiKhoanDAL.DoiTrangThai(maTaiKhoan, trangThai);
            TempData["ThongBao"] = "Đổi trạng thái thành công";
            return RedirectToAction(nameof(taiKhoan));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult donDepTaiKhoanKhongHoatDong()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            try
            {
                var maTaiKhoanDangNhap = HttpContext.Session.GetInt32("MaTaiKhoan");
                var ketQua = taiKhoanDAL.DonDepTaiKhoanKhongHoatDong(maTaiKhoanDangNhap);
                var coLoi = ketQua.ThongBao.StartsWith("Dọn tài khoản thất bại", StringComparison.OrdinalIgnoreCase);
                TempData["ThongBao"] = coLoi ? "Xóa thất bại." : "Xóa thành công.";
                TempData["LoaiThongBao"] = coLoi ? "danger" : "success";
            }
            catch
            {
                TempData["ThongBao"] = "Xóa thất bại.";
                TempData["LoaiThongBao"] = "danger";
            }

            return RedirectToAction(nameof(taiKhoan));
        }

        // Mục đích: action LaAdmin xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private bool LaAdmin()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return maTaiKhoan != null
                && (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(vaiTro, "Quản trị viên", StringComparison.OrdinalIgnoreCase));
        }

        private IActionResult? ChanNeuKhongPhaiAdmin()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            {
                TempData["ThongBao"] = "Vui lòng đăng nhập để tiếp tục";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!LaAdmin())
            {
                TempData["ThongBao"] = "Bạn không có quyền truy cập chức năng này";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction("Index", "NguoiDung");
            }

            return null;
        }
    }
}
