using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp VaccineController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class VaccineController : Controller
    {
        private readonly Vaccine_DAL vaccineDAL;

        public VaccineController(Vaccine_DAL vaccineDAL)
        {
            this.vaccineDAL = vaccineDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index(string? tuKhoa)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var danhSach = vaccineDAL.LayDanhSach();
            // Lọc danh sách vaccine theo tên vaccine hoặc nhóm vaccine, không thay đổi cấu trúc database.
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var tuKhoaTimKiem = tuKhoa.Trim();
                danhSach = danhSach
                    .Where(vaccine =>
                        vaccine.TenVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase)
                        || vaccine.NhomVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.TuKhoa = tuKhoa;
            return View(danhSach);
        }

        // Mục đích: action TraCuu xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult TraCuu(string? tuKhoa)
        {
            var danhSach = vaccineDAL.LayDanhSachDangSuDung();
            // Trang tra cứu chỉ lọc dữ liệu đã lấy từ bảng Vaccine, không thêm bảng/cột mới.
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var tuKhoaTimKiem = tuKhoa.Trim();
                danhSach = danhSach
                    .Where(vaccine =>
                        vaccine.TenVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase)
                        || vaccine.NhomVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.LaTraCuu = true;
            ViewBag.TuKhoa = tuKhoa;
            return View("Index", danhSach);
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int id)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var vaccine = vaccineDAL.LayTheoId(id);
            if (vaccine == null) return NotFound();

            return View(vaccine);
        }

        [HttpGet]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            return View(new Vaccine { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(Vaccine vaccine)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            if (!KiemTraHopLe(vaccine)) return View(vaccine);

            if (!vaccineDAL.Them(vaccine))
            {
                ViewBag.ThongBao = "Thêm vaccine thất bại, vui lòng thử lại";
                return View(vaccine);
            }

            TempData["ThongBao"] = "Thêm vaccine thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(int id)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var vaccine = vaccineDAL.LayTheoId(id);
            if (vaccine == null) return NotFound();

            return View(vaccine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(Vaccine vaccine)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            if (!KiemTraHopLe(vaccine)) return View(vaccine);

            if (!vaccineDAL.CapNhat(vaccine))
            {
                ViewBag.ThongBao = "Cập nhật vaccine thất bại, vui lòng thử lại";
                return View(vaccine);
            }

            TempData["ThongBao"] = "Cập nhật vaccine thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // Mục đích: action DeleteConfirmed xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult DeleteConfirmed(int id)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            try
            {
                if (!vaccineDAL.XoaHoacAn(id))
                {
                    TempData["ThongBao"] = "Không thể xóa vaccine này";
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                TempData["ThongBao"] = "Không thể xóa vaccine này vì đang có dữ liệu liên quan";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ThongBao"] = "Không thể xóa vaccine này";
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            TempData["ThongBao"] = "Xóa vaccine thành công";
            TempData["LoaiThongBao"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // Mục đích: action KiemTraHopLe xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private bool KiemTraHopLe(Vaccine vaccine)
        {
            if (string.IsNullOrWhiteSpace(vaccine.TenVaccine))
            {
                ViewBag.ThongBao = "Tên vaccine không được bỏ trống";
                return false;
            }

            if (vaccine.DoTuoiToiThieu.HasValue && vaccine.DoTuoiToiThieu.Value < 0)
            {
                ViewBag.ThongBao = "Độ tuổi tối thiểu không được âm";
                return false;
            }

            if (vaccine.DoTuoiToiDa.HasValue
                && vaccine.DoTuoiToiThieu.HasValue
                && vaccine.DoTuoiToiDa.Value < vaccine.DoTuoiToiThieu.Value)
            {
                ViewBag.ThongBao = "Độ tuổi tối đa phải lớn hơn hoặc bằng độ tuổi tối thiểu";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(vaccine.DonViTuoi))
            {
                var donViHopLe = new[] { "ngày", "tháng", "năm" };
                if (!donViHopLe.Contains(vaccine.DonViTuoi.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.ThongBao = "Đơn vị tuổi chỉ gồm: ngày, tháng, năm";
                    return false;
                }
            }

            return true;
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
