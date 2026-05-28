using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp MuiTiemVaccineController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class MuiTiemVaccineController : Controller
    {
        private readonly MuiTiemVaccine_DAL muiTiemVaccineDAL;
        private readonly Vaccine_DAL vaccineDAL;

        public MuiTiemVaccineController(MuiTiemVaccine_DAL muiTiemVaccineDAL, Vaccine_DAL vaccineDAL)
        {
            this.muiTiemVaccineDAL = muiTiemVaccineDAL;
            this.vaccineDAL = vaccineDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(muiTiemVaccineDAL.LayDanhSach());
        }

        // Mục đích: action IndexTheoVaccine xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult IndexTheoVaccine(int maVaccine)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var vaccine = vaccineDAL.LayTheoId(maVaccine);
            ViewBag.TenVaccine = vaccine?.TenVaccine ?? string.Empty;
            ViewBag.MaVaccine = maVaccine;
            return View(muiTiemVaccineDAL.LayDanhSachTheoVaccine(maVaccine));
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            return View(muiTiem);
        }

        [HttpGet]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(int? maVaccine)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            NapDanhSachVaccine(maVaccine);
            return View(new MuiTiemVaccine { MaVaccine = maVaccine ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(MuiTiemVaccine mt)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            if (!KiemTraHopLe(mt, null))
            {
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            if (!muiTiemVaccineDAL.Them(mt))
            {
                ViewBag.ThongBao = "Thêm mũi tiêm thất bại, vui lòng thử lại";
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            TempData["ThongBao"] = "Thêm mũi tiêm vaccine thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            NapDanhSachVaccine(muiTiem.MaVaccine);
            return View(muiTiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(MuiTiemVaccine mt)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            if (!KiemTraHopLe(mt, mt.MaMuiTiem))
            {
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            if (!muiTiemVaccineDAL.CapNhat(mt))
            {
                ViewBag.ThongBao = "Cập nhật mũi tiêm thất bại, vui lòng thử lại";
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            TempData["ThongBao"] = "Cập nhật mũi tiêm vaccine thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action Delete xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Delete(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            return View(muiTiem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // Mục đích: action DeleteConfirmed xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult DeleteConfirmed(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            muiTiemVaccineDAL.Xoa(id);
            TempData["ThongBao"] = "Xóa mũi tiêm vaccine thành công";
            return RedirectToAction(nameof(Index));
        }

        // Mục đích: action NapDanhSachVaccine xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private void NapDanhSachVaccine(int? maVaccineDangChon = null)
        {
            ViewBag.DanhSachVaccine = new SelectList(
                vaccineDAL.LayDanhSachDangSuDung(),
                "MaVaccine",
                "TenVaccine",
                maVaccineDangChon);
        }

        // Mục đích: action KiemTraHopLe xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private bool KiemTraHopLe(MuiTiemVaccine mt, int? maMuiTiemBoQua)
        {
            if (mt.MaVaccine <= 0)
            {
                ViewBag.ThongBao = "Vui lòng chọn vaccine";
                return false;
            }

            if (mt.SoMui <= 0)
            {
                ViewBag.ThongBao = "Số mũi không được bỏ trống và phải lớn hơn 0";
                return false;
            }

            if (muiTiemVaccineDAL.KiemTraTrungSoMui(mt.MaVaccine, mt.SoMui, maMuiTiemBoQua))
            {
                ViewBag.ThongBao = "Số mũi đã tồn tại trong vaccine này";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(mt.DonViTuoi))
            {
                var donViHopLe = new[] { "ngày", "tháng", "năm" };
                if (!donViHopLe.Contains(mt.DonViTuoi.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.ThongBao = "Đơn vị tuổi chỉ gồm: ngày, tháng, năm";
                    return false;
                }
            }

            if (mt.KhoangCachNgay.HasValue && mt.KhoangCachNgay.Value < 0)
            {
                ViewBag.ThongBao = "Khoảng cách ngày không được âm";
                return false;
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
            return maTaiKhoan != null
                && string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
