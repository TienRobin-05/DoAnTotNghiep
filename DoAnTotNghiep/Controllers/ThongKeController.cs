using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ThongKe_DAL thongKeDAL;

        public ThongKeController(ThongKe_DAL thongKeDAL)
        {
            this.thongKeDAL = thongKeDAL;
        }

        // hiển thị thống kê
        public IActionResult Index()
        {
            if (!LaAdmin()) return RedirectToAction("DangNhap", "TaiKhoan");
            return View(thongKeDAL.LayThongKeTongQuan());
        }

        private bool LaAdmin()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return maTaiKhoan != null && vaiTro == "Admin";
        }
    }
}
