using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    // Lớp AdminBaiVietController làa controller tniếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng

    public class AdminBaiVietController : Controller
    {
        private readonly BaiVietCamNang_DAL baiVietDAL;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AdminBaiVietController(BaiVietCamNang_DAL baiVietDAL, IWebHostEnvironment webHostEnvironment)
        {
            this.baiVietDAL = baiVietDAL;
            this.webHostEnvironment = webHostEnvironment;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác
        public IActionResult Index()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            return View(baiVietDAL.LayTatCaChoAdmin());
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
            return View(new BaiVietCamNang { TrangThai = true, LoaiBaiViet = "Cẩm nang" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(BaiVietCamNang model, IFormFile? anhDaiDienFile)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            model.LoaiBaiViet = BaiVietCamNang_DAL.ChuanHoaLoaiBaiViet(model.LoaiBaiViet) ?? string.Empty;
            if (!KiemTraHopLe(model))
            {
                return View(model);
            }

            var anhDaiDien = LuuAnhDaiDien(anhDaiDienFile);
            if (anhDaiDien == null && ViewBag.ThongBao != null)
            {
                return View(model);
            }

            model.AnhDaiDien = anhDaiDien ?? string.Empty;
            model.MaTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            model.NgayTao = DateTime.Now;

            if (!baiVietDAL.Them(model))
            {
                ViewBag.ThongBao = "Thêm bài viết thất bại, vui lòng thử lại.";
                return View(model);
            }

            TempData["ThongBao"] = "Thêm bài viết thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(int maBaiViet)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var baiViet = baiVietDAL.LayTheoId(maBaiViet);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(BaiVietCamNang model, IFormFile? anhDaiDienFile)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            model.LoaiBaiViet = BaiVietCamNang_DAL.ChuanHoaLoaiBaiViet(model.LoaiBaiViet) ?? string.Empty;
            if (!KiemTraHopLe(model))
            {
                return View(model);
            }

            var anhDaiDien = LuuAnhDaiDien(anhDaiDienFile);
            if (anhDaiDien == null && ViewBag.ThongBao != null)
            {
                return View(model);
            }

            if (!string.IsNullOrEmpty(anhDaiDien))
            {
                model.AnhDaiDien = anhDaiDien;
            }

            if (!baiVietDAL.CapNhat(model))
            {
                ViewBag.ThongBao = "Cập nhật bài viết thất bại, vui lòng thử lại.";
                return View(model);
            }

            TempData["ThongBao"] = "Cập nhật bài viết thành công";
            return RedirectToAction(nameof(Index));
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int maBaiViet)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var baiViet = baiVietDAL.LayTheoId(maBaiViet);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action DeleteConfirmed xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult DeleteConfirmed(int maBaiViet)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            baiVietDAL.Xoa(maBaiViet);
            TempData["ThongBao"] = "Xóa bài viết thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action AnHien xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult AnHien(int maBaiViet, bool trangThai)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            baiVietDAL.AnHien(maBaiViet, trangThai);
            TempData["ThongBao"] = "Đổi trạng thái thành công";
            return RedirectToAction(nameof(Index));
        }

        // Mục đích: action KiemTraHopLe xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private bool KiemTraHopLe(BaiVietCamNang model)
        {
            if (string.IsNullOrWhiteSpace(model.TieuDe))
            {
                ViewBag.ThongBao = "Tiêu đề không được bỏ trống.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.NoiDung))
            {
                ViewBag.ThongBao = "Nội dung không được bỏ trống.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.LoaiBaiViet))
            {
                ViewBag.ThongBao = "Vui lòng chọn loại bài viết.";
                return false;
            }

            return true;
        }

        private string? LuuAnhDaiDien(IFormFile? anhDaiDienFile)
        {
            if (anhDaiDienFile == null || anhDaiDienFile.Length == 0)
            {
                return null;
            }

            const long dungLuongToiDa = 2 * 1024 * 1024;
            if (anhDaiDienFile.Length > dungLuongToiDa)
            {
                ViewBag.ThongBao = "Ảnh đại diện không được vượt quá 2MB.";
                return null;
            }

            var duoiFile = Path.GetExtension(anhDaiDienFile.FileName).ToLowerInvariant();
            var duoiHopLe = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!duoiHopLe.Contains(duoiFile))
            {
                ViewBag.ThongBao = "Ảnh đại diện chỉ hỗ trợ JPG, PNG hoặc WEBP.";
                return null;
            }

            var thuMucUpload = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "bai-viet");
            Directory.CreateDirectory(thuMucUpload);

            var tenFile = $"{Guid.NewGuid():N}{duoiFile}";
            var duongDanVatLy = Path.Combine(thuMucUpload, tenFile);
            using var stream = new FileStream(duongDanVatLy, FileMode.Create);
            anhDaiDienFile.CopyTo(stream);

            return $"/uploads/bai-viet/{tenFile}";
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
