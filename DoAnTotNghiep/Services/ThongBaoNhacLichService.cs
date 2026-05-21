namespace DoAnTotNghiep.Services
{
    /// <summary>
    /// Lớp ThongBaoNhacLichService chứa nghiệp vụ dùng chung, giúp Controller/DAL tách riêng phần xử lý phức tạp khỏi luồng request chính.
    /// </summary>
    public class ThongBaoNhacLichService
    {
        // Mục đích: phương thức KiemTraVaTaoThongBaoNhacLich xử lý nghiệp vụ trung gian để Controller có thể tái sử dụng mà không lặp code.
        // Dữ liệu đầu vào: các model, mã định danh hoặc dữ liệu nghiệp vụ cần thiết cho quá trình xử lý.
        // Xử lý chính: phối hợp các bước tính toán, kiểm tra điều kiện và gọi DAL khi cần truy xuất dữ liệu.
        // Kết quả trả về: kết quả nghiệp vụ sau khi xử lý, có thể là dữ liệu, trạng thái thành công hoặc không trả về giá trị nếu chỉ thực hiện tác vụ.
        public void KiemTraVaTaoThongBaoNhacLich(int maTaiKhoan)
        {
            // Bảng ThongBao hiện tại không có maLichTiem, nên chức năng thông báo chỉ đọc dữ liệu có sẵn theo maTaiKhoan.
        }
    }
}
