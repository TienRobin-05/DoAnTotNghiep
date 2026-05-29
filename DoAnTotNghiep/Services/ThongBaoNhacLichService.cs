using DoAnTotNghiep.DAL;

namespace DoAnTotNghiep.Services
{
    /// <summary>
    /// Lớp tương thích cho các nơi còn dùng service; nghiệp vụ tạo thông báo nằm trong ThongBao_DAL.
    /// </summary>
    public class ThongBaoNhacLichService
    {
        private readonly ThongBao_DAL thongBaoDAL;

        public ThongBaoNhacLichService(ThongBao_DAL thongBaoDAL)
        {
            this.thongBaoDAL = thongBaoDAL;
        }

        // Tạo thông báo lịch tiêm đến hạn/quá hạn, không tạo trùng.
        public void KiemTraVaTaoThongBaoNhacLich(int maTaiKhoan)
        {
            thongBaoDAL.TaoThongBaoLichTiemDenHan(maTaiKhoan);
        }
    }
}
