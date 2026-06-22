using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DoAnTotNghiep.Controllers
{
    [ApiController]
    public class QuickQuestionsController : Controller
    {
        private readonly QuickQuestion_DAL quickQuestionDAL;
        private readonly CauHoiTuVan_DAL cauHoiTuVanDAL;

        public QuickQuestionsController(QuickQuestion_DAL quickQuestionDAL, CauHoiTuVan_DAL cauHoiTuVanDAL)
        {
            this.quickQuestionDAL = quickQuestionDAL;
            this.cauHoiTuVanDAL = cauHoiTuVanDAL;
        }

        [HttpGet("/api/quick-questions")]
        public IActionResult LayDanhSach([FromQuery(Name = "keyword")] string? keyword)
        {
            var danhSach = quickQuestionDAL.LayDanhSachDangBat(keyword);
            return Json(new
            {
                data = danhSach.Select(quickQuestion => new
                {
                    id = quickQuestion.Id,
                    question = quickQuestion.Question,
                    content = quickQuestion.Content,
                    topic = quickQuestion.Topic,
                    badge_label = string.IsNullOrWhiteSpace(quickQuestion.BadgeLabel) ? quickQuestion.Topic : quickQuestion.BadgeLabel,
                    sort_order = quickQuestion.SortOrder,
                    is_active = quickQuestion.IsActive
                })
            });
        }

        [HttpPost("/api/consultations/quick")]
        [IgnoreAntiforgeryToken]
        public IActionResult GuiNhanh([FromBody] QuickQuestionSendRequest request)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null)
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập để tiếp tục" });
            }

            var quickQuestion = quickQuestionDAL.LayTheoId(request.QuickQuestionId);
            if (quickQuestion == null || !quickQuestion.IsActive)
            {
                return NotFound(new { message = "Câu hỏi nhanh không tồn tại hoặc đã tắt" });
            }

            var now = DateTime.Now;
            var cauHoi = new CauHoiTuVan
            {
                MaNguoiGui = maTaiKhoan.Value,
                MaNguoiTraLoi = null,
                MaVaccine = null,
                CauHoi = $"Chủ đề: {quickQuestion.Topic}\nTiêu đề: {quickQuestion.Question}\n\n{quickQuestion.Content}",
                CauTraLoi = string.Empty,
                NgayGui = now,
                NgayTraLoi = null,
                TrangThai = "Chưa trả lời"
            };

            var id = cauHoiTuVanDAL.GuiCauHoiVaLayId(cauHoi);
            var code = $"HQ-{now:yyyyMMdd}-{id:00000}";

            return Json(new
            {
                data = new
                {
                    id,
                    code,
                    title = quickQuestion.Question,
                    topic = quickQuestion.Topic,
                    status = "processing",
                    created_at = now
                },
                message = "Đã gửi câu hỏi nhanh thành công"
            });
        }

        [HttpGet("/api/admin/quick-questions")]
        public IActionResult AdminLayDanhSach(string? keyword, string? topic, bool? is_active, int page = 1, int limit = 10)
        {
            if (!LaAdmin()) return Forbid();

            var danhSach = quickQuestionDAL.LayDanhSach(keyword, topic, is_active);
            page = Math.Max(1, page);
            limit = Math.Clamp(limit, 1, 100);
            var total = danhSach.Count;
            var data = danhSach.Skip((page - 1) * limit).Take(limit).ToList();

            return Json(new
            {
                data = data.Select(FormatQuickQuestion),
                pagination = new { page, limit, total }
            });
        }

        [HttpPost("/api/admin/quick-questions")]
        [IgnoreAntiforgeryToken]
        public IActionResult AdminThem([FromBody] QuickQuestion model)
        {
            if (!LaAdmin()) return Forbid();
            var validation = KiemTraHopLe(model);
            if (validation != null) return BadRequest(new { message = validation });

            quickQuestionDAL.Them(model, HttpContext.Session.GetInt32("MaTaiKhoan"));
            return Json(new { message = "Thêm câu hỏi nhanh thành công" });
        }

        [HttpPut("/api/admin/quick-questions/{id:int}")]
        [IgnoreAntiforgeryToken]
        public IActionResult AdminSua(int id, [FromBody] QuickQuestion model)
        {
            if (!LaAdmin()) return Forbid();
            var validation = KiemTraHopLe(model);
            if (validation != null) return BadRequest(new { message = validation });
            if (quickQuestionDAL.LayTheoId(id) == null) return NotFound(new { message = "Không tìm thấy câu hỏi nhanh" });

            model.Id = id;
            quickQuestionDAL.CapNhat(model, HttpContext.Session.GetInt32("MaTaiKhoan"));
            return Json(new { message = "Cập nhật câu hỏi nhanh thành công" });
        }

        [HttpPatch("/api/admin/quick-questions/{id:int}/status")]
        [IgnoreAntiforgeryToken]
        public IActionResult AdminCapNhatTrangThai(int id, [FromBody] QuickQuestionStatusRequest request)
        {
            if (!LaAdmin()) return Forbid();
            if (quickQuestionDAL.LayTheoId(id) == null) return NotFound(new { message = "Không tìm thấy câu hỏi nhanh" });

            quickQuestionDAL.CapNhatTrangThai(id, request.IsActive, HttpContext.Session.GetInt32("MaTaiKhoan"));
            return Json(new { message = request.IsActive ? "Đã bật câu hỏi nhanh" : "Đã tắt câu hỏi nhanh" });
        }

        [HttpDelete("/api/admin/quick-questions/{id:int}")]
        [IgnoreAntiforgeryToken]
        public IActionResult AdminXoa(int id)
        {
            if (!LaAdmin()) return Forbid();
            if (!quickQuestionDAL.Xoa(id)) return NotFound(new { message = "Không tìm thấy câu hỏi nhanh" });

            return Json(new { message = "Xóa câu hỏi nhanh thành công" });
        }

        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "User") return null;

            return maTaiKhoan.Value;
        }

        private bool LaAdmin()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return maTaiKhoan != null
                && (string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(vaiTro, "Quản trị viên", StringComparison.OrdinalIgnoreCase));
        }

        private static string? KiemTraHopLe(QuickQuestion model)
        {
            if (string.IsNullOrWhiteSpace(model.Question)) return "Câu hỏi hiển thị không được bỏ trống";
            if (string.IsNullOrWhiteSpace(model.Content)) return "Nội dung gửi đi không được bỏ trống";
            if (string.IsNullOrWhiteSpace(model.Topic)) return "Chủ đề không được bỏ trống";
            return null;
        }

        private static object FormatQuickQuestion(QuickQuestion quickQuestion)
        {
            return new
            {
                id = quickQuestion.Id,
                question = quickQuestion.Question,
                content = quickQuestion.Content,
                topic = quickQuestion.Topic,
                badge_label = string.IsNullOrWhiteSpace(quickQuestion.BadgeLabel) ? quickQuestion.Topic : quickQuestion.BadgeLabel,
                sort_order = quickQuestion.SortOrder,
                is_active = quickQuestion.IsActive,
                created_at = quickQuestion.CreatedAt,
                updated_at = quickQuestion.UpdatedAt
            };
        }
    }

    public class QuickQuestionSendRequest
    {
        [JsonPropertyName("quick_question_id")]
        public int QuickQuestionId { get; set; }
    }

    public class QuickQuestionStatusRequest
    {
        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }
    }
}
