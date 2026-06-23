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

        public IActionResult Index(string? keyword, string? status, string? topic, string? sort, int page = 1, int pageSize = 5)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            bool? isActive = status switch
            {
                "visible" => true,
                "hidden" => false,
                _ => null
            };

            var allItems = quickQuestionDAL.LayDanhSach(keyword, topic, isActive);

            allItems = sort switch
            {
                "oldest" => allItems.OrderBy(x => x.UpdatedAt ?? x.CreatedAt).ToList(),
                "az" => allItems.OrderBy(x => x.Question).ToList(),
                "updated" => allItems.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt).ToList(),
                _ => allItems.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt).ToList()
            };

            var totalItems = allItems.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalItems / pageSize));
            page = Math.Clamp(page, 1, totalPages);

            var pagedItems = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new AdminQuickQuestionIndexViewModel
            {
                TotalQuestions = quickQuestionDAL.DemTheoTrangThai(null),
                VisibleQuestions = quickQuestionDAL.DemTheoTrangThai(true),
                HiddenQuestions = quickQuestionDAL.DemTheoTrangThai(false),
                Keyword = keyword,
                SelectedStatus = status,
                SelectedTopic = topic,
                SelectedSort = sort,
                Topics = quickQuestionDAL.LayDanhSachChuDe(),
                Items = pagedItems.Select(x => new AdminQuickQuestionItemViewModel
                {
                    Id = x.Id,
                    QuestionCode = $"HQ-{x.CreatedAt:yyyyMMdd}-{x.Id:00000}",
                    Title = x.Question,
                    TopicName = x.Topic,
                    IsVisible = x.IsActive,
                    UpdatedAt = x.UpdatedAt ?? x.CreatedAt
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                StartItem = totalItems == 0 ? 0 : (page - 1) * pageSize + 1,
                EndItem = Math.Min(page * pageSize, totalItems),
                TotalPages = totalPages
            };

            return View(model);
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
        public IActionResult Toggle(int id, string? returnUrl)
        {
            var chanQuyen = ChanNeuKhongPhaiAdmin();
            if (chanQuyen != null) return chanQuyen;

            var quickQuestion = quickQuestionDAL.LayTheoId(id);
            if (quickQuestion == null) return NotFound();

            quickQuestionDAL.CapNhatTrangThai(id, !quickQuestion.IsActive, HttpContext.Session.GetInt32("MaTaiKhoan"));
            TempData["ThongBao"] = quickQuestion.IsActive ? "Đã tắt câu hỏi nhanh" : "Đã bật câu hỏi nhanh";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
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
