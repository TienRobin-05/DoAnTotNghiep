using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp HoSoSucKhoeController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class HoSoSucKhoeController : Controller
    {
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly TaiKhoan_DAL taiKhoanDAL;
        private readonly ThongBao_DAL thongBaoDAL;
        private readonly TaoLichTiemService taoLichTiemService;

        public HoSoSucKhoeController(
            HoSoSucKhoe_DAL hoSoSucKhoeDAL,
            TaiKhoan_DAL taiKhoanDAL,
            ThongBao_DAL thongBaoDAL,
            TaoLichTiemService taoLichTiemService)
        {
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.taiKhoanDAL = taiKhoanDAL;
            this.thongBaoDAL = thongBaoDAL;
            this.taoLichTiemService = taoLichTiemService;
        }

        // Helper: sinh lịch tiêm + lịch demo + đồng bộ notification
        private void SinhLichVaDongBoThongBao(int maHoSo, int maTaiKhoan)
        {
            System.Console.WriteLine($"[CreateProfile] Bat dau sinh lich cho maHoSo={maHoSo}, maTaiKhoan={maTaiKhoan}");

            // Bước 1: tạo lịch tiêm theo quy tắc chuẩn
            var ketQua = taoLichTiemService.TaoLichTiemChoHoSo(maHoSo);
            System.Console.WriteLine($"[CreateProfile] Da tao {ketQua.SoLichTiemDaTao} lich chuan, " +
                $"phuHop={ketQua.SoMuiTiemPhuHop}/{ketQua.SoMuiTiemVaccine}");

            // Bước 2: tạo lịch tiêm demo sắp đến hạn (hôm nay + 3 ngày) để test "Đến lịch"
            var demoTao = taoLichTiemService.TaoLichTiemDemoSapToi(maHoSo);
            if (demoTao > 0)
            {
                System.Console.WriteLine($"[CreateProfile] Da tao lich demo sap den han cho maHoSo={maHoSo}");
            }
            else
            {
                System.Console.WriteLine($"[CreateProfile] Lich demo da ton tai hoac khong the tao cho maHoSo={maHoSo}");
            }

            // Bước 3: đồng bộ notification
            var soTB = thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan);
            System.Console.WriteLine($"[CreateProfile] Da dong bo {soTB} thong bao cho maTaiKhoan={maTaiKhoan}");
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            CapNhatSoThongBaoLenMenu(maTaiKhoan.Value);
            var danhSach = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            return View(danhSach);
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int id)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            CapNhatSoThongBaoLenMenu(maTaiKhoan.Value);
            var hoSo = hoSoSucKhoeDAL.LayTheoId(id, maTaiKhoan.Value);
            if (hoSo == null) return NotFound();

            return View(hoSo);
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

            CapNhatSoThongBaoLenMenu(maTaiKhoan.Value);
            return View(new HoSoSucKhoe { NgaySinh = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Create xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Create(HoSoSucKhoe hoSo)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            // Parse NgaySinh từ hidden input (YYYY-MM-DD) - model binder không tự parse được
            var ngaySinhRaw = Request.Form["NgaySinh"].FirstOrDefault() ?? "";
            if (string.IsNullOrWhiteSpace(ngaySinhRaw))
            {
                ModelState.AddModelError("NgaySinh", "Vui lòng nhập ngày sinh hợp lệ theo định dạng MM/DD/YYYY.");
                ViewBag.ThongBao = "Vui lòng nhập ngày sinh hợp lệ.";
                return View(hoSo);
            }

            if (!DateTime.TryParseExact(ngaySinhRaw, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime ngaySinhParsed))
            {
                ModelState.AddModelError("NgaySinh", "Ngày sinh không đúng định dạng YYYY-MM-DD.");
                ViewBag.ThongBao = "Ngày sinh không hợp lệ.";
                return View(hoSo);
            }

            if (ngaySinhParsed > DateTime.Today)
            {
                ModelState.AddModelError("NgaySinh", "Ngày sinh không được lớn hơn ngày hiện tại.");
                ViewBag.ThongBao = "Ngày sinh không được lớn hơn ngày hiện tại.";
                return View(hoSo);
            }

            if (ngaySinhParsed.Year < 1900)
            {
                ModelState.AddModelError("NgaySinh", "Năm sinh không hợp lệ.");
                ViewBag.ThongBao = "Năm sinh không hợp lệ.";
                return View(hoSo);
            }

            hoSo.NgaySinh = ngaySinhParsed;

            if (!KiemTraHopLe(hoSo)) return View(hoSo);

            hoSo.MaTaiKhoan = maTaiKhoan.Value;
            hoSo.NgayTao = DateTime.Now;

            var maHoSoMoi = hoSoSucKhoeDAL.ThemVaLayId(hoSo);
            if (maHoSoMoi <= 0)
            {
                ViewBag.ThongBao = "Thêm hồ sơ thất bại, vui lòng thử lại";
                return View(hoSo);
            }

            // Sau khi tạo hồ sơ: sinh lịch tiêm + lịch demo + đồng bộ thông báo
            SinhLichVaDongBoThongBao(maHoSoMoi, maTaiKhoan.Value);
            TempData["ThongBao"] = "Thêm hồ sơ sức khỏe thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(int id)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            CapNhatSoThongBaoLenMenu(maTaiKhoan.Value);
            var hoSo = hoSoSucKhoeDAL.LayTheoId(id, maTaiKhoan.Value);
            if (hoSo == null) return NotFound();

            return View(hoSo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action Edit xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Edit(HoSoSucKhoe hoSo)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (!KiemTraHopLe(hoSo)) return View(hoSo);

            hoSo.MaTaiKhoan = maTaiKhoan.Value;
            if (!hoSoSucKhoeDAL.CapNhat(hoSo))
            {
                ViewBag.ThongBao = "Cập nhật hồ sơ thất bại hoặc hồ sơ không thuộc tài khoản của bạn";
                return View(hoSo);
            }

            TempData["ThongBao"] = "Cập nhật hồ sơ sức khỏe thành công";
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
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (hoSoSucKhoeDAL.Xoa(id, maTaiKhoan.Value))
            {
                TempData["ThongBao"] = "Xóa hồ sơ thành công";
                TempData["LoaiThongBao"] = "success";
            }
            else
            {
                TempData["ThongBao"] = "Xóa hồ sơ thất bại, vui lòng thử lại";
                TempData["LoaiThongBao"] = "error";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // Mục đích: action CapNhatThongTinCaNhan xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult CapNhatThongTinCaNhan()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (hoSoSucKhoeDAL.KiemTraTaiKhoanDaCoHoSo(maTaiKhoan.Value))
            {
                return RedirectToAction("Index", "NguoiDung");
            }

            GanThongTinTaiKhoanLenView(maTaiKhoan.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Mục đích: action CapNhatThongTinCaNhan xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult CapNhatThongTinCaNhan(
            string hoTen,
            DateTime? ngaySinh,
            string gioiTinh,
            double? chieuCao,
            double? canNang,
            string tienSuBenh,
            string diUng)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (hoSoSucKhoeDAL.KiemTraTaiKhoanDaCoHoSo(maTaiKhoan.Value))
            {
                return RedirectToAction("Index", "NguoiDung");
            }

            var hoSo = new HoSoSucKhoe
            {
                MaTaiKhoan = maTaiKhoan.Value,
                HoTen = hoTen,
                NgaySinh = ngaySinh ?? default,
                GioiTinh = gioiTinh ?? string.Empty,
                ChieuCao = chieuCao,
                CanNang = canNang,
                TienSuBenh = tienSuBenh ?? string.Empty,
                DiUng = diUng ?? string.Empty,
                NgayTao = DateTime.Now
            };

            if (!KiemTraHopLe(hoSo, ngaySinh))
            {
                GanThongTinTaiKhoanLenView(maTaiKhoan.Value);
                return View();
            }

            var maHoSoMoi = hoSoSucKhoeDAL.ThemVaLayId(hoSo);
            if (maHoSoMoi > 0)
            {
                taiKhoanDAL.CapNhatHoTen(maTaiKhoan.Value, hoTen);
                HttpContext.Session.SetString("HoTen", hoTen);
                // Hồ sơ cá nhân đầu tiên: sinh lịch tiêm + lịch demo + đồng bộ thông báo
                SinhLichVaDongBoThongBao(maHoSoMoi, maTaiKhoan.Value);
                TempData["ThongBao"] = "Cập nhật hồ sơ sức khỏe thành công";
                return RedirectToAction("Index", "NguoiDung");
            }

            GanThongTinTaiKhoanLenView(maTaiKhoan.Value);
            ViewBag.ThongBao = "Lưu thông tin cá nhân thất bại, vui lòng thử lại";
            return View();
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
            if (!string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase)) return null;

            return maTaiKhoan.Value;
        }

        // Mục đích: action KiemTraHopLe xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private bool KiemTraHopLe(HoSoSucKhoe hoSo, DateTime? ngaySinhNhap = null)
        {
            if (string.IsNullOrWhiteSpace(hoSo.HoTen))
            {
                ViewBag.ThongBao = "Họ tên không được bỏ trống";
                return false;
            }

            var ngaySinh = ngaySinhNhap ?? hoSo.NgaySinh;
            if (ngaySinh == default)
            {
                ViewBag.ThongBao = "Ngày sinh không được bỏ trống";
                return false;
            }

            if (ngaySinh.Date > DateTime.Today)
            {
                ViewBag.ThongBao = "Ngày sinh không được lớn hơn ngày hiện tại";
                return false;
            }

            if (hoSo.ChieuCao.HasValue && hoSo.ChieuCao.Value <= 0)
            {
                ViewBag.ThongBao = "Chiều cao phải lớn hơn 0";
                return false;
            }

            if (hoSo.CanNang.HasValue && hoSo.CanNang.Value <= 0)
            {
                ViewBag.ThongBao = "Cân nặng phải lớn hơn 0";
                return false;
            }

            return true;
        }

        // Mục đích: action GanThongTinTaiKhoanLenView xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private void GanThongTinTaiKhoanLenView(int maTaiKhoan)
        {
            var soDienThoai = HttpContext.Session.GetString("SoDienThoai");
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                var taiKhoan = taiKhoanDAL.LayTaiKhoanTheoId(maTaiKhoan);
                soDienThoai = taiKhoan?.SoDienThoai ?? string.Empty;
                email = taiKhoan?.Email ?? string.Empty;
            }

            ViewBag.SoDienThoai = soDienThoai;
            ViewBag.Email = email;
        }

        // Cập nhật số thông báo chưa đọc cho menu dùng _UserLayout.
        private void CapNhatSoThongBaoLenMenu(int maTaiKhoan)
        {
            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan);
        }
    }
}
