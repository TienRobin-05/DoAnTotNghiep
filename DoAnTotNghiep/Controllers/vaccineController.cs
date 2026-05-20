using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class VaccineController : Controller
    {
        private readonly Vaccine_DAL vaccineDAL;

        public VaccineController(Vaccine_DAL vaccineDAL)
        {
            this.vaccineDAL = vaccineDAL;
        }

        public IActionResult Index()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(vaccineDAL.LayDanhSach());
        }

        public IActionResult TraCuu()
        {
            ViewBag.LaTraCuu = true;
            return View("Index", vaccineDAL.LayDanhSachDangSuDung());
        }

        public IActionResult Details(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var vaccine = vaccineDAL.LayTheoId(id);
            if (vaccine == null) return NotFound();

            return View(vaccine);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(new Vaccine { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vaccine vaccine)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            if (!KiemTraHopLe(vaccine)) return View(vaccine);

            if (!vaccineDAL.Them(vaccine))
            {
                ViewBag.ThongBao = "Thêm vaccine thất bại, vui lòng thử lại";
                return View(vaccine);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var vaccine = vaccineDAL.LayTheoId(id);
            if (vaccine == null) return NotFound();

            return View(vaccine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Vaccine vaccine)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            if (!KiemTraHopLe(vaccine)) return View(vaccine);

            if (!vaccineDAL.CapNhat(vaccine))
            {
                ViewBag.ThongBao = "Cập nhật vaccine thất bại, vui lòng thử lại";
                return View(vaccine);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var vaccine = vaccineDAL.LayTheoId(id);
            if (vaccine == null) return NotFound();

            return View(vaccine);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            vaccineDAL.XoaHoacAn(id);
            return RedirectToAction(nameof(Index));
        }

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
                var donViHopLe = new[] { "ngày", "tháng", "năm" };
                if (!donViHopLe.Contains(vaccine.DonViTuoi.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.ThongBao = "Đơn vị tuổi chỉ gồm: ngày, tháng, năm";
                    return false;
                }
            }

            return true;
        }

        private bool LaAdmin()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            return maTaiKhoan != null
                && string.Equals(HttpContext.Session.GetString("VaiTro"), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
