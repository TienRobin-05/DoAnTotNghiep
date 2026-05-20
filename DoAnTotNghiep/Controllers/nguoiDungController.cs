using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class nguoiDungController : Controller
    {
        private readonly thongBaoDAL thongBaoDAL;
        private readonly lichTiemDAL lichTiemDAL;

        public nguoiDungController(thongBaoDAL thongBaoDAL, lichTiemDAL lichTiemDAL)
        {
            this.thongBaoDAL = thongBaoDAL;
            this.lichTiemDAL = lichTiemDAL;
        }

        public IActionResult index()
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null)
            {
                return RedirectToAction("dangNhap", "taiKhoan");
            }

            ViewBag.ThongBao = thongBaoDAL.layTheoTaiKhoan(maTaiKhoan.Value).Take(5).ToList();
            ViewBag.LichTiem = lichTiemDAL.layTheoTaiKhoan(maTaiKhoan.Value).Take(5).ToList();
            return View();
        }

        private int? layMaTaiKhoan()
        {
            return HttpContext.Session.GetInt32("MaTaiKhoan");
        }
    }
}
