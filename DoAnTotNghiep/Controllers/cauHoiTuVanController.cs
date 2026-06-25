using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class CauHoiTuVanController : Controller
    {
        private readonly CauHoiTuVan_DAL cauHoiTuVanDAL;
        private readonly Vaccine_DAL vaccineDAL;

        public CauHoiTuVanController(CauHoiTuVan_DAL cauHoiTuVanDAL, Vaccine_DAL vaccineDAL)
        {
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
            this.vaccineDAL = vaccineDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index(string? keyword, string? trangThai, string? topic, string? sort, int page = 1, int pageSize = 10)
        {
            if (LaAdmin())
            {
                var danhSachGoc = cauHoiTuVanDAL.LayTatCa();
                var danhSach = danhSachGoc.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var tuKhoa = keyword.Trim();
                    danhSach = danhSach.Where(item =>
                        item.MaCauHoi.ToString().Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                        || item.CauHoi.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                        || item.CauTraLoi.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                        || item.TenNguoiGui.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                        || item.TenVaccine.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase)
                        || item.NhomVaccine.Contains(tuKhoa, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(trangThai))
                {
                    danhSach = trangThai switch
                    {
                        "answered" => danhSach.Where(DaTraLoi),
                        "pending" => danhSach.Where(item => !DaTraLoi(item)),
                        _ => danhSach
                    };
                }

                if (!string.IsNullOrWhiteSpace(topic))
                {
                    danhSach = danhSach.Where(item =>
                        string.Equals(item.TenVaccine, topic, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(item.NhomVaccine, topic, StringComparison.OrdinalIgnoreCase));
                }

                danhSach = string.Equals(sort, "oldest", StringComparison.OrdinalIgnoreCase)
                    ? danhSach.OrderBy(item => item.NgayGui)
                    : danhSach.OrderByDescending(item => item.NgayGui);

                pageSize = pageSize is 20 or 50 ? pageSize : 10;
                var tongKetQua = danhSach.Count();
                var tongTrang = Math.Max(1, (int)Math.Ceiling(tongKetQua / (double)pageSize));
                page = Math.Clamp(page, 1, tongTrang);
                var batDau = tongKetQua == 0 ? 0 : ((page - 1) * pageSize) + 1;
                var ketThuc = Math.Min(page * pageSize, tongKetQua);

                ViewBag.LaAdmin = true;
                ViewBag.Keyword = keyword ?? string.Empty;
                ViewBag.TrangThai = trangThai ?? string.Empty;
                ViewBag.Topic = topic ?? string.Empty;
                ViewBag.Sort = string.IsNullOrWhiteSpace(sort) ? "newest" : sort;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = tongKetQua;
                ViewBag.TotalPages = tongTrang;
                ViewBag.StartItem = batDau;
                ViewBag.EndItem = ketThuc;
                ViewBag.TotalQuestions = danhSachGoc.Count;
                ViewBag.AnsweredQuestions = danhSachGoc.Count(DaTraLoi);
                ViewBag.PendingQuestions = danhSachGoc.Count(item => !DaTraLoi(item));
                ViewBag.Topics = danhSachGoc
                    .Select(item => string.IsNullOrWhiteSpace(item.TenVaccine) ? item.NhomVaccine : item.TenVaccine)
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(item => item)
                    .ToList();

                return View(danhSach.Skip((page - 1) * pageSize).Take(pageSize).ToList());
            }

            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            ViewBag.LaAdmin = false;
            return View(cauHoiTuVanDAL.LayDanhSachTheoNguoiGui(maTaiKhoan.Value));
        }

        [HttpGet]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            return View(new CauHoiTuVan());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(CauHoiTuVan model)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (!model.MaVaccine.HasValue || model.MaVaccine.Value <= 0)
            {
                ViewBag.ThongBao = "Vui lòng chọn vaccine cần tư vấn.";
                return View(model);
            }

            var vaccine = vaccineDAL.LayTheoId(model.MaVaccine.Value);
            if (vaccine == null || !vaccine.TrangThai)
            {
                ViewBag.ThongBao = "Vaccine đã chọn không còn trong danh sách đang dùng.";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.CauHoi))
            {
                ViewBag.ThongBao = "Vui lòng nhập nội dung câu hỏi.";
                return View(model);
            }

            var cauHoi = new CauHoiTuVan
            {
                MaNguoiGui = maTaiKhoan.Value,
                MaNguoiTraLoi = null,
                MaVaccine = vaccine.MaVaccine,
                CauHoi = model.CauHoi.Trim(),
                CauTraLoi = string.Empty,
                NgayGui = DateTime.Now,
                NgayTraLoi = null,
                TrangThai = "Chưa trả lời"
            };

            if (!cauHoiTuVanDAL.GuiCauHoi(cauHoi))
            {
                ViewBag.ThongBao = "Gửi câu hỏi thất bại, vui lòng thử lại.";
                return View(model);
            }

            TempData["ThongBao"] = "Gửi câu hỏi tư vấn thành công";
            return RedirectToAction(nameof(Index));
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int maCauHoi)
        {
            if (LaAdmin())
            {
                var cauHoiAdmin = cauHoiTuVanDAL.LayTheoId(maCauHoi);
                return cauHoiAdmin == null ? NotFound() : View(cauHoiAdmin);
            }

            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var cauHoi = cauHoiTuVanDAL.LayTheoIdCuaNguoiGui(maCauHoi, maTaiKhoan.Value);
            return cauHoi == null ? NotFound() : View(cauHoi);
        }

        [HttpGet]
        public IActionResult traLoi(int id, int maCauHoi = 0)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var maCauHoiCanTraLoi = maCauHoi > 0 ? maCauHoi : id;
            var cauHoi = cauHoiTuVanDAL.LayTheoId(maCauHoiCanTraLoi);
            return cauHoi == null ? NotFound() : View(cauHoi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult traLoi(int maCauHoi, string cauTraLoi)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (string.IsNullOrWhiteSpace(cauTraLoi))
            {
                var cauHoi = cauHoiTuVanDAL.LayTheoId(maCauHoi);
                ViewBag.ThongBao = "Vui lòng nhập câu trả lời.";
                return cauHoi == null ? NotFound() : View(cauHoi);
            }

            cauHoiTuVanDAL.TraLoi(maCauHoi, maTaiKhoan.Value, cauTraLoi.Trim());
            TempData["ThongBao"] = "Cập nhật câu hỏi tư vấn thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action guiCauHoi xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult guiCauHoi()
        {
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action guiCauHoi xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult guiCauHoi(CauHoiTuVan model)
        {
            return Create(model);
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

        private bool LaAdmin()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return HttpContext.Session.GetInt32("MaTaiKhoan") != null
                && (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(vaiTro, "Quản trị viên", StringComparison.OrdinalIgnoreCase));
        }

        private static bool DaTraLoi(CauHoiTuVan cauHoi)
        {
            return cauHoi.NgayTraLoi.HasValue
                || !string.IsNullOrWhiteSpace(cauHoi.CauTraLoi)
                || cauHoi.TrangThai.Contains("Đã", StringComparison.OrdinalIgnoreCase);
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
