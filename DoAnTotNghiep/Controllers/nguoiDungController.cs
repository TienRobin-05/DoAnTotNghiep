using DoAnTotNghiep.DAL;
using DoAnTotNghiep.Models;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    /// <summary>
    /// Lớp NguoiDungController là controller tiếp nhận request từ trình duyệt, gọi các lớp DAL hoặc service cần thiết và trả về View/Redirect phù hợp cho người dùng.
    /// </summary>
    public class NguoiDungController : Controller
    {
        private readonly ThongBao_DAL thongBaoDAL;
        private readonly HoSoSucKhoe_DAL hoSoSucKhoeDAL;
        private readonly LichTiem_DAL lichTiemDAL;

        public NguoiDungController(
            ThongBao_DAL thongBaoDAL,
            HoSoSucKhoe_DAL hoSoSucKhoeDAL,
            LichTiem_DAL lichTiemDAL)
        {
            this.thongBaoDAL = thongBaoDAL;
            this.hoSoSucKhoeDAL = hoSoSucKhoeDAL;
            this.lichTiemDAL = lichTiemDAL;
        }

        // Mục đích: action Index xử lý request tương ứng từ người dùng và quyết định trả về giao diện hoặc chuyển hướng phù hợp.
        // Dữ liệu đầu vào: dữ liệu gửi từ route, query string, form hoặc session tùy theo màn hình đang thao tác.
        // Xử lý chính: kiểm tra dữ liệu cần thiết, gọi DAL/service để đọc hoặc cập nhật dữ liệu, sau đó gán thông báo/ViewBag/TempData nếu cần.
        // Kết quả trả về: IActionResult là View hiển thị cho người dùng hoặc RedirectToAction khi cần chuyển sang màn hình khác.
        public IActionResult Index()
        {
            var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.Equals(vaiTro, "User", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            ViewBag.HoTen = HttpContext.Session.GetString("HoTen");
            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan.Value);
            ViewBag.SoThongBaoChuaDoc = thongBaoDAL.DemThongBaoChuaDoc(maTaiKhoan.Value);
            ViewBag.ThongBaoMoiNhat = thongBaoDAL.LayThongBaoMoiNhat(maTaiKhoan.Value, 4);

            var danhSachHoSo = hoSoSucKhoeDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            var tatCaLichTiem = lichTiemDAL.LayDanhSachTheoTaiKhoan(maTaiKhoan.Value);
            var lichTiemChuaTiem = tatCaLichTiem
                .Where(lich => lich.TrangThai != "Đã tiêm")
                .ToList();

            var lichTiemQuaHan = lichTiemChuaTiem
                .Where(lich => lich.NgayTiemDuKien.Date < DateTime.Today)
                .OrderBy(lich => lich.NgayTiemDuKien)
                .ToList();

            var lichTiemHomNay = lichTiemChuaTiem
                .Where(lich => lich.NgayTiemDuKien.Date == DateTime.Today)
                .OrderBy(lich => lich.TenVaccine)
                .ToList();

            var lichTiemSapToi = lichTiemChuaTiem
                .Where(lich => lich.NgayTiemDuKien.Date > DateTime.Today)
                .OrderBy(lich => lich.NgayTiemDuKien)
                .ToList();

            ViewBag.DanhSachHoSo = danhSachHoSo;
            ViewBag.LichTiemQuaHan = lichTiemQuaHan.Take(4).ToList();
            ViewBag.LichTiemHomNay = lichTiemHomNay.Take(4).ToList();
            ViewBag.LichTiemSapToi = lichTiemSapToi.Take(5).ToList();
            ViewBag.SoMuiQuaHan = lichTiemQuaHan.Count;
            ViewBag.SoMuiHomNay = lichTiemHomNay.Count;
            ViewBag.SoMuiSapDenHan = lichTiemSapToi.Count;
            ViewBag.SoMuiHoanThanh = tatCaLichTiem.Count(lich => lich.TrangThai == "Đã tiêm");
            return View();
        }
    }
}
