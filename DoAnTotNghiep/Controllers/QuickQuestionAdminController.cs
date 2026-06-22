using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    public class QuickQuestionAdminController : Controller
    {
        private readonly QuickQuestion_DAL quickQuestionDAL;

        public QuickQuestionAdminController(QuickQuestion_DAL quickQuestionDAL)
        {
            this.quickQuestionDAL = quickQuestionDAL;
        }

        public IActionResult Index(string? keyword, string? topic, string? trangThai)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            bool? isActive = trangThai switch
            {
                "active" => true,
                "inactive" => false,
                _ => null
            };

            ViewBag.Keyword = keyword ?? string.Empty;
            ViewBag.Topic = topic ?? string.Empty;
            ViewBag.TrangThai = trangThai ?? string.Empty;
            return View(quickQuestionDAL.LayDanhSach(keyword, topic, isActive));
        }

        [HttpGet]
        public IActionResult Create()
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            return View("Form", new QuickQuestion { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuickQuestion model)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            if (!KiemTraHopLe(model)) return View("Form", model);

            quickQuestionDAL.Them(model, HttpContext.Session.GetInt32("MaTaiKhoan"));
            TempData["ThongBao"] = "Thêm câu hỏi nhanh thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var quickQuestion = quickQuestionDAL.LayTheoId(id);
            return quickQuestion == null ? NotFound() : View("Form", quickQuestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(QuickQuestion model)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            if (!KiemTraHopLe(model)) return View("Form", model);

            quickQuestionDAL.CapNhat(model, HttpContext.Session.GetInt32("MaTaiKhoan"));
            TempData["ThongBao"] = "Cập nhật câu hỏi nhanh thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Toggle(int id)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var quickQuestion = quickQuestionDAL.LayTheoId(id);
            if (quickQuestion == null) return NotFound();

            quickQuestionDAL.CapNhatTrangThai(id, !quickQuestion.IsActive, HttpContext.Session.GetInt32("MaTaiKhoan"));
            TempData["ThongBao"] = quickQuestion.IsActive ? "Đã tắt câu hỏi nhanh" : "Đã bật câu hỏi nhanh";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            quickQuestionDAL.Xoa(id);
            TempData["ThongBao"] = "Xóa câu hỏi nhanh thành công";
            return RedirectToAction(nameof(Index));
        }

        private bool KiemTraHopLe(QuickQuestion model)
        {
            if (string.IsNullOrWhiteSpace(model.Question))
            {
                ViewBag.ThongBao = "Câu hỏi hiển thị không được bỏ trống";
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Content))
            {
                ViewBag.ThongBao = "Nội dung gửi đi không được bỏ trống";
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Topic))
            {
                ViewBag.ThongBao = "Chủ đề không được bỏ trống";
                return false;
            }

            return true;
        }

        private bool LaAdmin()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return maTaiKhoan != null
                && (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(vaiTro, "Quản trị viên", StringComparison.OrdinalIgnoreCase));
        }

        private IActionResult? ChanNeuKhongPhaiAdmin()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            {
                TempData["ThongBao"] = "Vui lòng đăng nhập để tiếp tục";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!LaAdmin())
            {
                TempData["ThongBao"] = "Bạn không có quyền truy cập chức năng này";
                TempData["LoaiThongBao"] = "warning";
                return RedirectToAction("Index", "NguoiDung");
            }

            return null;
        }
    }
}
