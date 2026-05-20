using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class HoSoSucKhoeController : Controller
    {
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly TaiKhoan_DAL taiKhoanDAL;

        public HoSoSucKhoeController(HoSoSucKhoe_DAL hoSoSucKhoeDAL, TaiKhoan_DAL taiKhoanDAL)
        {
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.taiKhoanDAL = taiKhoanDAL;
        }

        public IActionResult Index()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var danhSach = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            return View(danhSach);
        }

        public IActionResult Details(int id)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var hoSo = hoSoSucKhoeDAL.LayTheoId(id, maTaiKhoan.Value);
            if (hoSo == null) return NotFound();

            return View(hoSo);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (LayMaTaiKhoanUser() == null) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(new HoSoSucKhoe { NgaySinh = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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
        public IActionResult DeleteConfirmed(int id)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            hoSoSucKhoeDAL.Xoa(id, maTaiKhoan.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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
                return RedirectToAction("Index", "NguoiDung");
            }

            GanThongTinTaiKhoanLenView(maTaiKhoan.Value);
            ViewBag.ThongBao = "Lưu thông tin cá nhân thất bại, vui lòng thử lại";
            return View();
        }

        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase)) return null;

            return maTaiKhoan.Value;
        }

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
