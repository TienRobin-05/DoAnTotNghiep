using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class Tai_KhoanController : Controller
    {
        private readonly Tai_Khoan_DAL _taiKhoanDAL;

        public Tai_KhoanController(Tai_Khoan_DAL taiKhoanDAL)
        {
            _taiKhoanDAL = taiKhoanDAL;
        }

        // GET: /Tai_Khoan/DangNhap
        [HttpGet]
        public IActionResult DangNhap()
        {
            return RedirectToAction("Index", "Home");
        }

        // POST: /Tai_Khoan/DangNhap
        [HttpPost]
        public IActionResult DangNhap(Login_View_Model model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ThongBao"] = "Vui lòng nhập đầy đủ email và mật khẩu";
                return RedirectToAction("Index", "Home");
            }

            Tai_Khoan? taiKhoan = _taiKhoanDAL.DangNhap(model.soDienThoai, model.matKhau);

            if (taiKhoan == null)
            {
                TempData["ThongBao"] = "Số điện thoại hoặc mật khẩu không đúng";
                return RedirectToAction("Index", "Home");
            }

            // Lưu thông tin đăng nhập vào Session
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.maTaiKhoan);
            HttpContext.Session.SetString("HoTen", taiKhoan.hoTen ?? "");
            HttpContext.Session.SetString("VaiTro", taiKhoan.vaiTro ?? "");
            HttpContext.Session.SetString("SoDienThoai", taiKhoan.soDienThoai ?? "");

            // Đăng nhập thành công thì chuyển vào form chính
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult DangKy(Dang_Ky_View_Model model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_taiKhoanDAL.EmailDaTonTai(model.email))
            {
                ViewBag.ThongBao = "Email này đã được đăng ký";
                return View(model);
            }

            bool ketQua = _taiKhoanDAL.DangKy(model);

            if (!ketQua)
            {
                ViewBag.ThongBao = "Không thể đăng ký tài khoản. Vui lòng thử lại";
                return View(model);
            }

            TempData["ThongBao"] = "Đăng ký tài khoản thành công. Vui lòng đăng nhập";
            TempData["HoTenDangKy"] = model.hoTen;
            return RedirectToAction("HoSoSauDangKy", "Tai_Khoan");
        }

        [HttpGet]
        public IActionResult HoSoSauDangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult HoSoSauDangKy(string hoTen, string gioiTinh, DateTime? ngaySinh)
        {
            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(gioiTinh) || ngaySinh == null)
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ họ tên, giới tính và ngày sinh";
                return View();
            }

            TempData["ThongBao"] = "Đã lưu thông tin hồ sơ. Vui lòng đăng nhập";
            return RedirectToAction("DangNhap", "Tai_Khoan");
        }

        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            // Kiểm tra đã đăng nhập chưa, chưa đăng nhập thì quay về trang chủ.
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

            if (maTaiKhoan == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy tài khoản hiện tại để hiển thị tên và số điện thoại.
            Tai_Khoan? taiKhoan = _taiKhoanDAL.LayTheoMa(maTaiKhoan.Value);

            if (taiKhoan == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }

            // Ngày sinh và giới tính hiện lưu tạm trong Session vì bảng TaiKhoan chưa có hai cột này.
            string? ngaySinhTrongSession = HttpContext.Session.GetString("NgaySinh");
            DateTime? ngaySinh = DateTime.TryParse(ngaySinhTrongSession, out DateTime ngaySinhDaLuu)
                ? ngaySinhDaLuu
                : null;

            var model = new Thong_Tin_Ca_Nhan_View_Model
            {
                hoTen = taiKhoan.hoTen,
                gioiTinh = HttpContext.Session.GetString("GioiTinh") ?? "",
                ngaySinh = ngaySinh,
                soDienThoai = taiKhoan.soDienThoai
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ThongTinCaNhan(Thong_Tin_Ca_Nhan_View_Model model)
        {
            // Chỉ tài khoản đã đăng nhập mới được lưu thông tin cá nhân.
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

            if (maTaiKhoan == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Luôn lấy số điện thoại từ database để người dùng không sửa được bằng form.
            Tai_Khoan? taiKhoan = _taiKhoanDAL.LayTheoMa(maTaiKhoan.Value);

            if (taiKhoan == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }

            model.soDienThoai = taiKhoan.soDienThoai;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Cập nhật họ tên vào bảng TaiKhoan.
            bool ketQua = _taiKhoanDAL.CapNhatHoTen(maTaiKhoan.Value, model.hoTen);

            if (!ketQua)
            {
                ViewBag.ThongBao = "Không thể cập nhật thông tin. Vui lòng thử lại";
                return View(model);
            }

            // Cập nhật lại Session để tên trên góc phải đổi ngay sau khi lưu.
            HttpContext.Session.SetString("HoTen", model.hoTen);
            HttpContext.Session.SetString("GioiTinh", model.gioiTinh);
            HttpContext.Session.SetString("NgaySinh", model.ngaySinh!.Value.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("SoDienThoai", taiKhoan.soDienThoai ?? "");

            ViewBag.ThanhCong = "Đã lưu thông tin cá nhân";
            return View(model);
        }

        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult QuenMatKhau(Quen_Mat_Khau_View_Model model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool ketQua = _taiKhoanDAL.DoiMatKhau(model.email, model.matKhauMoi);

            if (!ketQua)
            {
                ViewBag.ThongBao = "Không tìm thấy tài khoản đang hoạt động với email này";
                return View(model);
            }

            TempData["ThongBao"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại";
            return RedirectToAction("DangNhap", "Tai_Khoan");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap", "Tai_Khoan");
        }
    }
}
