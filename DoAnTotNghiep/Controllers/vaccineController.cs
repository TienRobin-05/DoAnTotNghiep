using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
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
        public IActionResult Index(string? tuKhoa, string? nhom, string? trangThai, int page = 1, int pageSize = 10)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var tatCaVaccine = vaccineDAL.LayDanhSach();
            var danhSachLoc = tatCaVaccine.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var tuKhoaTimKiem = tuKhoa.Trim();
                danhSachLoc = danhSachLoc
                    .Where(vaccine =>
                        vaccine.TenVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase)
                        || vaccine.NhomVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase)
                        || vaccine.MaVaccine.ToString().Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(nhom))
            {
                var nhomTimKiem = nhom.Trim();
                danhSachLoc = danhSachLoc.Where(vaccine =>
                    string.Equals(vaccine.NhomVaccine, nhomTimKiem, StringComparison.OrdinalIgnoreCase));
            }

            if (string.Equals(trangThai, "active", StringComparison.OrdinalIgnoreCase))
            {
                danhSachLoc = danhSachLoc.Where(vaccine => vaccine.TrangThai);
            }
            else if (string.Equals(trangThai, "inactive", StringComparison.OrdinalIgnoreCase))
            {
                danhSachLoc = danhSachLoc.Where(vaccine => !vaccine.TrangThai);
            }

            pageSize = new[] { 10, 20, 50 }.Contains(pageSize) ? pageSize : 10;
            var tongKetQua = danhSachLoc.Count();
            var tongTrang = Math.Max(1, (int)Math.Ceiling(tongKetQua / (double)pageSize));
            page = Math.Clamp(page, 1, tongTrang);

            var danhSach = danhSachLoc
                .OrderByDescending(vaccine => vaccine.MaVaccine)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new AdminVaccineIndexViewModel
            {
                TotalVaccines = tatCaVaccine.Count,
                ActiveVaccines = tatCaVaccine.Count(vaccine => vaccine.TrangThai),
                LastUpdatedText = $"Hôm nay, {DateTime.Now:HH:mm}",
                Keyword = tuKhoa,
                SelectedGroup = nhom,
                SelectedStatus = trangThai,
                VaccineGroups = tatCaVaccine
                    .Select(vaccine => vaccine.NhomVaccine)
                    .Where(nhomVaccine => !string.IsNullOrWhiteSpace(nhomVaccine))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(nhomVaccine => nhomVaccine)
                    .ToList(),
                Vaccines = danhSach,
                Page = page,
                PageSize = pageSize,
                TotalItems = tongKetQua,
                StartItem = tongKetQua == 0 ? 0 : ((page - 1) * pageSize) + 1,
                EndItem = Math.Min(page * pageSize, tongKetQua),
                TotalPages = tongTrang
            };

            ViewBag.TuKhoa = tuKhoa;
            return View(viewModel);
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

            var viewModel = new AdminVaccineIndexViewModel
            {
                TotalVaccines = danhSach.Count,
                ActiveVaccines = danhSach.Count,
                Keyword = tuKhoa,
                Vaccines = danhSach,
                Page = 1,
                PageSize = Math.Max(danhSach.Count, 1),
                TotalItems = danhSach.Count,
                StartItem = danhSach.Count == 0 ? 0 : 1,
                EndItem = danhSach.Count,
                TotalPages = 1
            };

            ViewBag.LaTraCuu = true;
            ViewBag.TuKhoa = tuKhoa;
            return View("Index", viewModel);
        }

        [HttpGet]
        public IActionResult DanhSachDangSuDungJson(string? tuKhoa, int limit = 1000)
        {
            var danhSach = vaccineDAL.LayDanhSachDangSuDung();
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var tuKhoaTimKiem = tuKhoa.Trim();
                danhSach = danhSach
                    .Where(vaccine =>
                        vaccine.TenVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase)
                        || vaccine.NhomVaccine.Contains(tuKhoaTimKiem, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (limit > 0)
            {
                danhSach = danhSach.Take(limit).ToList();
            }

            return Json(new
            {
                data = danhSach.Select(vaccine => new
                {
                    id = vaccine.MaVaccine,
                    name = vaccine.TenVaccine,
                    group = vaccine.NhomVaccine,
                    age_range = HienThiDoTuoi(vaccine),
                    status = vaccine.TrangThai ? "active" : "inactive",
                    status_label = vaccine.TrangThai ? "Đang dùng" : "Đã ẩn"
                })
            });
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

            if (vaccineDAL.KiemTraVaccineCoMuiTiem(id))
            {
                TempData["ThongBao"] = "Không thể xóa vaccine vì đã có dữ liệu mũi tiêm liên quan. Bạn có thể chuyển vaccine sang trạng thái ngừng sử dụng.";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(Index));
            }

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

            TempData["ThongBao"] = "Đã ngừng dùng vaccine";
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
                if (!DonViTuoiHelper.HopLe(vaccine.DonViTuoi))
                {
                    ViewBag.ThongBao = $"Đơn vị tuổi chỉ gồm: {DonViTuoiHelper.DanhSachDonViHopLe()}";
                    return false;
                }

                vaccine.DonViTuoi = DonViTuoiHelper.ChuanHoa(vaccine.DonViTuoi);
            }

            return true;
        }

        private static string HienThiDoTuoi(Vaccine vaccine)
        {
            if (!vaccine.DoTuoiToiThieu.HasValue && !vaccine.DoTuoiToiDa.HasValue)
            {
                return "Chưa cấu hình";
            }

            var toiThieu = vaccine.DoTuoiToiThieu?.ToString() ?? "0";
            var toiDa = vaccine.DoTuoiToiDa?.ToString() ?? "không giới hạn";
            var donVi = string.IsNullOrWhiteSpace(vaccine.DonViTuoi) ? string.Empty : $" {vaccine.DonViTuoi.Trim()}";
            return $"{toiThieu} - {toiDa}{donVi}";
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
