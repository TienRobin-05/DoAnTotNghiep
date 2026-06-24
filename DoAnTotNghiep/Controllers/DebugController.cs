using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class DebugController : Controller
    {
        private readonly TaoLichTiemService taoLichTiemService;
        private readonly ThongBao_DAL thongBaoDAL;
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;

        public DebugController(
            TaoLichTiemService taoLichTiemService,
            ThongBao_DAL thongBaoDAL,
            HoSoSucKhoe_DAL hoSoSucKhoeDAL)
        {
            this.taoLichTiemService = taoLichTiemService;
            this.thongBaoDAL = thongBaoDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
        }

        [HttpPost]
        public IActionResult GenerateSchedules(int maHoSo)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
                return Json(new { error = "Chua dang nhap" });

            var ketQua = taoLichTiemService.TaoLichTiemChoHoSo(maHoSo);
            var taoTB = thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);

            return Json(new
            {
                success = true,
                tongMuiTiemXuLy = ketQua.SoMuiTiemVaccine,
                soLichDaTao = ketQua.SoLichTiemDaTao,
                soMuiPhuHop = ketQua.SoMuiTiemPhuHop,
                thongBaoDaTao = taoTB
            });
        }

        [HttpPost]
        public IActionResult CreateDemoSchedule(int maHoSo)
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
                return Json(new { error = "Chua dang nhap" });

            var hoSo = hoSoSucKhoeDAL.LayTheoId(maHoSo, maTaiKhoan.Value);
            if (hoSo == null)
                return Json(new { error = "Khong tim thay ho so" });

            var taoLich = taoLichTiemService.TaoLichTiemChoHoSo(maHoSo);
            var taoTB = thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);

            return Json(new
            {
                success = true,
                message = $"Da xu ly {taoLich.SoMuiTiemVaccine} mui tiem, phu hop {taoLich.SoMuiTiemPhuHop}, tao {taoLich.SoLichTiemDaTao} lich moi.",
                thongBaoDaTao = taoTB
            });
            
        }
    }
}
