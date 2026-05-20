using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class hoSoSucKhoeController : Controller
    {
        private readonly hoSoSucKhoeDAL hoSoSucKhoeDAL;

        public hoSoSucKhoeController(hoSoSucKhoeDAL hoSoSucKhoeDAL)
        {
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
        }

        public IActionResult index()
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            return View(hoSoSucKhoeDAL.layTheoTaiKhoan(maTaiKhoan.Value));
        }

        [HttpGet]
        public IActionResult them()
        {
            if (layMaTaiKhoan() == null) return RedirectToAction("dangNhap", "taiKhoan");
            return View(new hoSoSucKhoeModels { ngaySinh = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(hoSoSucKhoeModels model)
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            model.maTaiKhoan = maTaiKhoan.Value;
            hoSoSucKhoeDAL.them(model);
            return RedirectToAction(nameof(index));
        }

        [HttpGet]
        public IActionResult sua(int id)
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            var hoSo = hoSoSucKhoeDAL.layTheoMa(id, maTaiKhoan.Value);
            return hoSo == null ? NotFound() : View(hoSo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(hoSoSucKhoeModels model)
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            if (!ModelState.IsValid) return View(model);
            model.maTaiKhoan = maTaiKhoan.Value;
            hoSoSucKhoeDAL.capNhat(model);
            return RedirectToAction(nameof(index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult xoa(int id)
        {
            var maTaiKhoan = layMaTaiKhoan();
            if (maTaiKhoan == null) return RedirectToAction("dangNhap", "taiKhoan");
            hoSoSucKhoeDAL.xoa(id, maTaiKhoan.Value);
            return RedirectToAction(nameof(index));
        }

        private int? layMaTaiKhoan()
        {
            return HttpContext.Session.GetInt32("MaTaiKhoan");
        }
    }
}
