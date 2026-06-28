using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : Controller
    {
        // hiển thị trang chủ
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChucNang(string id)
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var dichDen = id switch
            {
                "quan-ly-vaccine" => ("Index", "Vaccine"),
                "quan-ly-mui-tiem" => ("Index", "MuiTiemVaccine"),
                "quan-ly-bai-viet" => ("Index", "AdminBaiViet"),
                "quan-ly-tu-van" => ("Index", "CauHoiTuVan"),
                "ho-so-suc-khoe" => ("Index", "HoSoSucKhoe"),
                "lich-tiem" => ("ChonHoSo", "LichTiem"),
                "cap-nhat-tiem" => ("ChonHoSo", "LichTiem"),
                "lich-su-tiem" => ("ChonHoSo", "LichSuTiem"),
                "thong-bao" => ("Index", "ThongBao"),
                "tra-cuu-vaccine" => ("TraCuu", "Vaccine"),
                "hoi-dap-tu-van" => ("Index", "CauHoiTuVan"),
                _ => (string.Empty, string.Empty)
            };

            if (!string.IsNullOrEmpty(dichDen.Item1))
            {
                return RedirectToAction(dichDen.Item1, dichDen.Item2);
            }

            ViewBag.MaChucNang = id;
            return View();
        }

        // hiển thị trang riêng tư
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // hiển thị trang lỗi
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
