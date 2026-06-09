using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp BaiVietCamNangController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class BaiVietCamNangController : Controller
    {
        private readonly BaiVietCamNang_DAL baiVietDAL;

        public BaiVietCamNangController(BaiVietCamNang_DAL baiVietDAL)
        {
            this.baiVietDAL = baiVietDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index(string? loai)
        {
            var loaiDaChuanHoa = BaiVietCamNang_DAL.ChuanHoaLoaiBaiViet(loai);
            ViewBag.LoaiDangChon = loaiDaChuanHoa ?? "Tất cả";
            return View(baiVietDAL.LayDanhSachHienThiChoUser(loaiDaChuanHoa));
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int maBaiViet)
        {
            var baiViet = baiVietDAL.LayTheoId(maBaiViet);
            if (baiViet == null || !baiViet.TrangThai)
            {
                return NotFound();
            }

            return View(baiViet);
        }
    }
}
