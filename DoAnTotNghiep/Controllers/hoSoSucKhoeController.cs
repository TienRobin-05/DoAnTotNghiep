using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
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

        public HoSoSucKhoeController(HoSoSucKhoe_DAL hoSoSucKhoeDAL, TaiKhoan_DAL taiKhoanDAL)
        {
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.taiKhoanDAL = taiKhoanDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

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
            if (LayMaTaiKhoanUser() == null) return RedirectToAction("DangNhap", "TaiKhoan");
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

            if (!KiemTraHopLe(hoSo)) return View(hoSo);

            hoSo.MaTaiKhoan = maTaiKhoan.Value;
            hoSo.NgayTao = DateTime.Now;

            if (!hoSoSucKhoeDAL.Them(hoSo))
            {
                ViewBag.ThongBao = "Thêm hồ sơ thất bại, vui lòng thử lại";
                return View(hoSo);
            }

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

        [HttpGet]
        // Mục đích: action Delete xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Delete(int id)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var hoSo = hoSoSucKhoeDAL.LayTheoId(id, maTaiKhoan.Value);
            if (hoSo == null) return NotFound();

            return View(hoSo);
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
                TempData["ThongBao"] = "Xóa hồ sơ sức khỏe thành công";
            }
            else
            {
                TempData["ThongBao"] = "Xóa hồ sơ sức khỏe thất bại";
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

            if (hoSoSucKhoeDAL.Them(hoSo))
            {
                taiKhoanDAL.CapNhatHoTen(maTaiKhoan.Value, hoTen);
                HttpContext.Session.SetString("HoTen", hoTen);
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
    }
}
