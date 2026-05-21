using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp muiTiemVaccineDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class muiTiemVaccineDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public muiTiemVaccineDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức layTatCa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<muiTiemVaccineModels> layTatCa()
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select mt.maMuiTiem, mt.maVaccine, mt.soMui, mt.tenMui, mt.doTuoiToiThieu, mt.doTuoiToiDa,
mt.doTuoiKhuyenNghi, mt.donViTuoi, mt.khoangCachNgay, mt.ghiChu, v.tenVaccine
from MuiTiemVaccine mt
inner join Vaccine v on mt.maVaccine = v.maVaccine
order by mt.maMuiTiem desc";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<muiTiemVaccineModels>();
            while (doc.Read())
            {
                danhSach.Add(docMuiTiem(doc));
            }
            return danhSach;
        }

        // Mục đích: phương thức layTheoMa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public muiTiemVaccineModels? layTheoMa(int maMuiTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select mt.maMuiTiem, mt.maVaccine, mt.soMui, mt.tenMui, mt.doTuoiToiThieu, mt.doTuoiToiDa,
mt.doTuoiKhuyenNghi, mt.donViTuoi, mt.khoangCachNgay, mt.ghiChu, v.tenVaccine
from MuiTiemVaccine mt
inner join Vaccine v on mt.maVaccine = v.maVaccine
where mt.maMuiTiem = @maMuiTiem";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maMuiTiem", maMuiTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docMuiTiem(doc) : null;
        }

        // Mục đích: phương thức them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void them(muiTiemVaccineModels muiTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"insert into MuiTiemVaccine(maVaccine, soMui, tenMui, doTuoiToiThieu, doTuoiToiDa, doTuoiKhuyenNghi, donViTuoi, khoangCachNgay, ghiChu)
values(@maVaccine, @soMui, @tenMui, @doTuoiToiThieu, @doTuoiToiDa, @doTuoiKhuyenNghi, @donViTuoi, @khoangCachNgay, @ghiChu)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, muiTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức capNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void capNhat(muiTiemVaccineModels muiTiem)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"update MuiTiemVaccine
set maVaccine = @maVaccine, soMui = @soMui, tenMui = @tenMui, doTuoiToiThieu = @doTuoiToiThieu,
    doTuoiToiDa = @doTuoiToiDa, doTuoiKhuyenNghi = @doTuoiKhuyenNghi, donViTuoi = @donViTuoi,
    khoangCachNgay = @khoangCachNgay, ghiChu = @ghiChu
where maMuiTiem = @maMuiTiem";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, muiTiem);
            lenh.Parameters.AddWithValue("@maMuiTiem", muiTiem.maMuiTiem);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức ganThamSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static void ganThamSo(SqlCommand lenh, muiTiemVaccineModels muiTiem)
        {
            lenh.Parameters.AddWithValue("@maVaccine", muiTiem.maVaccine);
            lenh.Parameters.AddWithValue("@soMui", muiTiem.soMui);
            lenh.Parameters.AddWithValue("@tenMui", (object?)muiTiem.tenMui ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiThieu", (object?)muiTiem.doTuoiToiThieu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiDa", (object?)muiTiem.doTuoiToiDa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiKhuyenNghi", (object?)muiTiem.doTuoiKhuyenNghi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@donViTuoi", (object?)muiTiem.donViTuoi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@khoangCachNgay", (object?)muiTiem.khoangCachNgay ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@ghiChu", (object?)muiTiem.ghiChu ?? DBNull.Value);
        }

        // Mục đích: phương thức docMuiTiem thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static muiTiemVaccineModels docMuiTiem(SqlDataReader doc)
        {
            return new muiTiemVaccineModels
            {
                maMuiTiem = Convert.ToInt32(doc["maMuiTiem"]),
                maVaccine = Convert.ToInt32(doc["maVaccine"]),
                soMui = Convert.ToInt32(doc["soMui"]),
                tenMui = doc["tenMui"] == DBNull.Value ? null : doc["tenMui"].ToString(),
                doTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                doTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                doTuoiKhuyenNghi = doc["doTuoiKhuyenNghi"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiKhuyenNghi"]),
                donViTuoi = doc["donViTuoi"] == DBNull.Value ? null : doc["donViTuoi"].ToString(),
                khoangCachNgay = doc["khoangCachNgay"] == DBNull.Value ? null : Convert.ToInt32(doc["khoangCachNgay"]),
                ghiChu = doc["ghiChu"] == DBNull.Value ? null : doc["ghiChu"].ToString(),
                tenVaccine = doc["tenVaccine"].ToString()
            };
        }
    }
}
