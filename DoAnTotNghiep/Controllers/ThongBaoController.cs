using DoAnTotNghiep.DAL;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp ThongBaoController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class ThongBaoController : Controller
    {
        private readonly ThongBao_DAL thongBaoDAL;

        public ThongBaoController(ThongBao_DAL thongBaoDAL)
        {
            this.thongBaoDAL = thongBaoDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            return View(thongBaoDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value));
        }

        // Mục đích: action Details xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Details(int id, int maThongBao = 0)
        {
            var maTaiKhoan = LayMaTaiKhoanUser();
            if (maTaiKhoan == null) return RedirectToAction("DangNhap", "TaiKhoan");

            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            var maThongBaoCanXem = maThongBao > 0 ? maThongBao : id;
            var thongBao = thongBaoDAL.LayTheoId(maThongBaoCanXem, maTaiKhoan.Value);
            if (thongBao == null) return RedirectToAction(nameof(Index));

            if (!thongBao.DaDoc)
            {
                if (thongBaoDAL.DanhDauDaDoc(maThongBaoCanXem, maTaiKhoan.Value))
                {
                    TempData["ThongBao"] = "Đã đánh dấu thông báo là đã đọc";
                    thongBao.DaDoc = true;
                    ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
                }
                else
                {
                    TempData["ThongBao"] = "Cập nhật trạng thái thông báo thất bại";
                    TempData["LoaiThongBao"] = "error";
                }
            }

            return View(thongBao);
        }

        // Mục đích: action LayMaTaiKhoanUser xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        private int? LayMaTaiKhoanUser()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null) return null;

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "User") return null;

            return maTaiKhoan.Value;
        }
    }
}
