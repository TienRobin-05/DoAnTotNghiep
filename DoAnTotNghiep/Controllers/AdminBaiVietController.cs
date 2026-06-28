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

        // hiển thị danh sách bài viết (admin)
        public IActionResult Index()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            return View(baiVietDAL.LayTatCaChoAdmin());
        }

        [HttpGet]
        // hiển thị form thêm bài viết
        public IActionResult Create()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;
            return View(new BaiVietCamNang { TrangThai = true, LoaiBaiViet = "Cẩm nang sức khỏe" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // xử lý thêm bài viết
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        // đổi trạng thái nổi bật
        public IActionResult DoiNoiBat(int maBaiViet, bool noiBat)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            baiVietDAL.DoiNoiBat(maBaiViet, noiBat);
            TempData["ThongBao"] = "Đổi trạng thái nổi bật thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int maBaiViet)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var baiViet = baiVietDAL.LayTheoId(maBaiViet);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // xử lý sửa bài viết
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

        public IActionResult Details(int maBaiViet)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var baiViet = baiVietDAL.LayTheoId(maBaiViet);
            return baiViet == null ? NotFound() : View(baiViet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // xóa bài viết
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
        // ẩn/hiện bài viết
        public IActionResult AnHien(int maBaiViet, bool trangThai)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            baiVietDAL.AnHien(maBaiViet, trangThai);
            TempData["ThongBao"] = "Đổi trạng thái thành công";
            return RedirectToAction(nameof(Index));
        }

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

            if (string.IsNullOrWhiteSpace(model.MoTaNgan))
            {
                ViewBag.ThongBao = "Mô tả ngắn không được bỏ trống.";
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

            if (!LaTepAnhHopLe(anhDaiDienFile, duoiFile))
            {
                ViewBag.ThongBao = "Tệp tải lên không đúng định dạng ảnh.";
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

        private static bool LaTepAnhHopLe(IFormFile file, string duoiFile)
        {
            var contentType = (file.ContentType ?? string.Empty).ToLowerInvariant();
            var mimeHopLe = duoiFile switch
            {
                ".jpg" or ".jpeg" => contentType is "image/jpeg" or "image/pjpeg",
                ".png" => contentType == "image/png",
                ".webp" => contentType == "image/webp",
                _ => false
            };

            if (!mimeHopLe)
            {
                return false;
            }

            Span<byte> header = stackalloc byte[12];
            using var stream = file.OpenReadStream();
            var soByteDoc = stream.Read(header);

            return duoiFile switch
            {
                ".jpg" or ".jpeg" => soByteDoc >= 3
                    && header[0] == 0xFF
                    && header[1] == 0xD8
                    && header[2] == 0xFF,
                ".png" => soByteDoc >= 8
                    && header[0] == 0x89
                    && header[1] == 0x50
                    && header[2] == 0x4E
                    && header[3] == 0x47
                    && header[4] == 0x0D
                    && header[5] == 0x0A
                    && header[6] == 0x1A
                    && header[7] == 0x0A,
                ".webp" => soByteDoc >= 12
                    && header[0] == 0x52
                    && header[1] == 0x49
                    && header[2] == 0x46
                    && header[3] == 0x46
                    && header[8] == 0x57
                    && header[9] == 0x45
                    && header[10] == 0x42
                    && header[11] == 0x50,
                _ => false
            };
        }

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
