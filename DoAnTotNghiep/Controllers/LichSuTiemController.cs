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

        // Mục đích: action ChonHoSo xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
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

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
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
    }
}
