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
        private readonly TaoLichTiemService taoLichTiemService;

        public LichTiemController(
            LichTiem_DAL lichTiemDAL,
            LichSuTiem_DAL lichSuTiemDAL,
            HoSoSucKhoe_DAL hoSoSucKhoeDAL,
            TaoLichTiemService taoLichTiemService)
        {
            this.lichTiemDAL = lichTiemDAL;
            this.lichSuTiemDAL = lichSuTiemDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
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

            var danhSachHoSo = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
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

            // Chá»‰ cho user xem lá»‹ch tiÃªm cá»§a há»“ sÆ¡ thuá»™c tÃ i khoáº£n Ä‘ang Ä‘Äƒng nháº­p.
            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null) return NotFound();

            // Náº¿u há»“ sÆ¡ chÆ°a cÃ³ lá»‹ch, há»‡ thá»‘ng tá»± táº¡o lá»‹ch dá»± kiáº¿n tá»« phÃ¡c Ä‘á»“ mÅ©i tiÃªm vaccine.
            if (!lichTiemDAL.KiemTraHoSoCoLichTiem(maHoSo))
            {
                taoLichTiemService.TaoLichTiemChoHoSo(maHoSo);
            }

            ViewBag.HoTenHoSo = hoSo.HoTen;
            return View(lichTiemDAL.LayDanhSachTheoHoSo(maHoSo, maTaiKhoan.Value));
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
                TempData["ThongBao"] = "Lá»‹ch tiÃªm nÃ y Ä‘Ã£ Ä‘Æ°á»£c cáº­p nháº­t lá»‹ch sá»­ tiÃªm.";
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

            // NgÃ y tiÃªm thá»±c táº¿ pháº£i náº±m trong khoáº£ng tá»« ngÃ y sinh Ä‘áº¿n ngÃ y hiá»‡n táº¡i.
            if (ngayTiemThucTe.Date < lichTiem.NgaySinhHoSo.Date)
            {
                ViewBag.ThongBao = "NgÃ y tiÃªm thá»±c táº¿ khÃ´ng Ä‘Æ°á»£c nhá» hÆ¡n ngÃ y sinh cá»§a há»“ sÆ¡.";
                ViewBag.NgayTiemThucTe = ngayTiemThucTe.ToString("yyyy-MM-dd");
                ViewBag.GhiChu = ghiChu;
                return View(lichTiem);
            }

            if (ngayTiemThucTe.Date > DateTime.Today)
            {
                ViewBag.ThongBao = "NgÃ y tiÃªm thá»±c táº¿ khÃ´ng Ä‘Æ°á»£c lá»›n hÆ¡n ngÃ y hiá»‡n táº¡i.";
                ViewBag.NgayTiemThucTe = ngayTiemThucTe.ToString("yyyy-MM-dd");
                ViewBag.GhiChu = ghiChu;
                return View(lichTiem);
            }

            // KhÃ´ng táº¡o láº¡i lá»‹ch sá»­ náº¿u lá»‹ch tiÃªm nÃ y Ä‘Ã£ Ä‘Æ°á»£c ghi nháº­n trÆ°á»›c Ä‘Ã³.
            if (lichSuTiemDAL.KiemTraDaCoLichSu(maLichTiem))
            {
                TempData["ThongBao"] = "Lá»‹ch tiÃªm nÃ y Ä‘Ã£ Ä‘Æ°á»£c cáº­p nháº­t lá»‹ch sá»­ tiÃªm.";
                return RedirectToAction(nameof(Index), new { maHoSo = lichTiem.MaHoSo });
            }

            lichTiemDAL.CapNhatDaTiem(maLichTiem);
            lichSuTiemDAL.Them(new LichSuTiem
            {
                MaLichTiem = maLichTiem,
                NgayTiemThucTe = ngayTiemThucTe,
                GhiChu = ghiChu ?? string.Empty,
                NgayCapNhat = DateTime.Now
            });

            TempData["ThongBao"] = "Cáº­p nháº­t tráº¡ng thÃ¡i Ä‘Ã£ tiÃªm thÃ nh cÃ´ng.";
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
    }
}
