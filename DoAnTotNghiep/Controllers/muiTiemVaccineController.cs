using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoAnTotNghiep.Controllers
{
    public class MuiTiemVaccineController : Controller
    {
        private readonly MuiTiemVaccine_DAL muiTiemVaccineDAL;
        private readonly Vaccine_DAL vaccineDAL;

        public MuiTiemVaccineController(MuiTiemVaccine_DAL muiTiemVaccineDAL, Vaccine_DAL vaccineDAL)
        {
            this.muiTiemVaccineDAL = muiTiemVaccineDAL;
            this.vaccineDAL = vaccineDAL;
        }

        public IActionResult Index()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(muiTiemVaccineDAL.LayDanhSach());
        }

        public IActionResult IndexTheoVaccine(int maVaccine)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var vaccine = vaccineDAL.LayTheoId(maVaccine);
            ViewBag.TenVaccine = vaccine?.TenVaccine ?? string.Empty;
            ViewBag.MaVaccine = maVaccine;
            return View(muiTiemVaccineDAL.LayDanhSachTheoVaccine(maVaccine));
        }

        public IActionResult Details(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            return View(muiTiem);
        }

        [HttpGet]
        public IActionResult Create(int? maVaccine)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            NapDanhSachVaccine(maVaccine);
            return View(new MuiTiemVaccine { MaVaccine = maVaccine ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MuiTiemVaccine mt)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            if (!KiemTraHopLe(mt, null))
            {
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            if (!muiTiemVaccineDAL.Them(mt))
            {
                ViewBag.ThongBao = "Thêm mũi tiêm thất bại, vui lòng thử lại";
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            NapDanhSachVaccine(muiTiem.MaVaccine);
            return View(muiTiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MuiTiemVaccine mt)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            if (!KiemTraHopLe(mt, mt.MaMuiTiem))
            {
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            if (!muiTiemVaccineDAL.CapNhat(mt))
            {
                ViewBag.ThongBao = "Cập nhật mũi tiêm thất bại, vui lòng thử lại";
                NapDanhSachVaccine(mt.MaVaccine);
                return View(mt);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");

            var muiTiem = muiTiemVaccineDAL.LayTheoId(id);
            if (muiTiem == null) return NotFound();

            return View(muiTiem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            muiTiemVaccineDAL.Xoa(id);
            return RedirectToAction(nameof(Index));
        }

        private void NapDanhSachVaccine(int? maVaccineDangChon = null)
        {
            ViewBag.DanhSachVaccine = new SelectList(
                vaccineDAL.LayDanhSachDangSuDung(),
                "MaVaccine",
                "TenVaccine",
                maVaccineDangChon);
        }

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
                var donViHopLe = new[] { "ngày", "tháng", "năm" };
                if (!donViHopLe.Contains(mt.DonViTuoi.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    ViewBag.ThongBao = "Đơn vị tuổi chỉ gồm: ngày, tháng, năm";
                    return false;
                }
            }

            if (mt.KhoangCachNgay.HasValue && mt.KhoangCachNgay.Value < 0)
            {
                ViewBag.ThongBao = "Khoảng cách ngày không được âm";
                return false;
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
