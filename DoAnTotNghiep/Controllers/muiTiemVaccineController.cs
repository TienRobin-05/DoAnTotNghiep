using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
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

        public IActionResult Index(string? keyword, string? intervalType, int page = 1, int pageSize = 10)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var allItems = muiTiemVaccineDAL.LayTatCaMuiTiemKemVaccine();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLowerInvariant();
                allItems = allItems.Where(x =>
                    (x.TenVaccine ?? "").ToLowerInvariant().Contains(kw) ||
                    (x.TenMui ?? "").ToLowerInvariant().Contains(kw) ||
                    x.MaMuiTiem.ToString().Contains(kw)).ToList();
            }

            if (intervalType == "age")
            {
                allItems = allItems.Where(x => x.KhoangCachNgay == null || x.KhoangCachNgay == 0).ToList();
            }
            else if (intervalType == "days")
            {
                allItems = allItems.Where(x => x.KhoangCachNgay.HasValue && x.KhoangCachNgay.Value > 0).ToList();
            }

            var totalItems = allItems.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalItems / pageSize));
            page = Math.Clamp(page, 1, totalPages);

            var pagedItems = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new AdminDoseIndexViewModel
            {
                TotalDoses = totalItems,
                Keyword = keyword,
                SelectedIntervalType = intervalType,
                Items = pagedItems.Select(x => new AdminDoseItemViewModel
                {
                    Id = x.MaMuiTiem,
                    VaccineId = x.MaVaccine,
                    VaccineName = x.TenVaccine,
                    DoseNumber = x.SoMui,
                    DoseName = x.TenMui,
                    RecommendedScheduleText = HienThiDoTuoi(x),
                    IntervalText = HienThiKhoangCach(x)
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                StartItem = totalItems == 0 ? 0 : (page - 1) * pageSize + 1,
                EndItem = Math.Min(page * pageSize, totalItems),
                TotalPages = totalPages
            };

            return View(model);
        }

        private static string HienThiDoTuoi(MuiTiemVaccine x)
        {
            if (x.DoTuoiKhuyenNghi.HasValue)
                return $"{x.DoTuoiKhuyenNghi} {x.DonViTuoi}";
            if (x.DoTuoiToiThieu.HasValue && x.DoTuoiToiDa.HasValue)
                return $"{x.DoTuoiToiThieu}-{x.DoTuoiToiDa} {x.DonViTuoi}";
            if (x.DoTuoiToiThieu.HasValue)
                return $"Từ {x.DoTuoiToiThieu} {x.DonViTuoi}";
            if (x.DoTuoiToiDa.HasValue)
                return $"Đến {x.DoTuoiToiDa} {x.DonViTuoi}";
            return "Chưa cấu hình";
        }

        private static string HienThiKhoangCach(MuiTiemVaccine x)
        {
            if (x.KhoangCachNgay.HasValue && x.KhoangCachNgay.Value > 0)
                return $"{x.KhoangCachNgay} ngày";
            return "Theo tuổi";
        }

        // Mục đích: action IndexTheoVaccine xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult IndexTheoVaccine(int maVaccine)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var vaccine = vaccineDAL.LayTheoId(maVaccine);
            if (vaccine == null) return NotFound();

            ViewBag.TenVaccine = vaccine?.TenVaccine ?? string.Empty;
            ViewBag.MaVaccine = maVaccine;
            return View(muiTiemVaccineDAL.LayMuiTiemTheoVaccine(maVaccine));
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int id)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            ViewBag.DaDuocSuDungTrongLichTiem = muiTiemVaccineDAL.KiemTraDaCoLichTiemSuDung(id);
            return View(muiTiem);
        }

        [HttpGet]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(int? maVaccine)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            if (maVaccine.HasValue && vaccineDAL.LayTheoId(maVaccine.Value) == null)
            {
                return NotFound();
            }

            NapDanhSachVaccine(maVaccine);
            ViewBag.KhoaVaccine = maVaccine.HasValue;
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
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            if (!KiemTraHopLe(mt, null))
            {
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            if (!muiTiemVaccineDAL.ThemMuiTiemVaccine(mt))
            {
                ViewBag.ThongBao = "Thêm mũi tiêm thất bại, vui lòng thử lại";
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            TempData["ThongBao"] = "Thêm mũi tiêm vaccine thành công";
            return RedirectToAction(nameof(IndexTheoVaccine), new { maVaccine = mt.MaVaccine });
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
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            if (!KiemTraHopLe(mt, mt.MaMuiTiem))
            {
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            if (!muiTiemVaccineDAL.SuaMuiTiemVaccine(mt))
            {
                ViewBag.ThongBao = "Cập nhật mũi tiêm thất bại, vui lòng thử lại";
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            TempData["ThongBao"] = "Cập nhật mũi tiêm vaccine thành công";
            return RedirectToAction(nameof(IndexTheoVaccine), new { maVaccine = mt.MaVaccine });
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
            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            if (muiTiemVaccineDAL.KiemTraDaCoLichTiemSuDung(id))
            {
                TempData["ThongBao"] = "Không thể xóa mũi tiêm này vì đã được dùng trong lịch tiêm. Bạn có thể sửa thông tin mũi tiêm nếu cần.";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction(nameof(IndexTheoVaccine), new { maVaccine = muiTiem.MaVaccine });
            }

            if (!muiTiemVaccineDAL.XoaMuiTiemVaccine(id))
            {
                TempData["ThongBao"] = "Xóa mũi tiêm vaccine thất bại, vui lòng thử lại";
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction(nameof(IndexTheoVaccine), new { maVaccine = muiTiem.MaVaccine });
            }

            TempData["ThongBao"] = "Xóa mũi tiêm vaccine thành công";
            TempData["LoaiThongBao"] = "success";
            return RedirectToAction(nameof(IndexTheoVaccine), new { maVaccine = muiTiem.MaVaccine });
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
                if (!DonViTuoiHelper.HopLe(mt.DonViTuoi))
                {
                    ViewBag.ThongBao = $"Đơn vị tuổi chỉ gồm: {DonViTuoiHelper.DanhSachDonViHopLe()}";
                    return false;
                }

                mt.DonViTuoi = DonViTuoiHelper.ChuanHoa(mt.DonViTuoi);
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
