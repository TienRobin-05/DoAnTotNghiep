using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    /// <summary>
    /// Lớp vaccineDAL chịu trách nhiệm truy cập dữ liệu cho chức năng liên quan, bao gồm mở kết nối SQL Server, truyền tham số an toàn và chuyển dữ liệu database thành model.
    /// </summary>
    public class vaccineDAL
    {
        private readonly coSoDuLieu coSoDuLieu;

        public vaccineDAL(coSoDuLieu coSoDuLieu)
        {
            this.coSoDuLieu = coSoDuLieu;
        }

        // Mục đích: phương thức layTatCa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public List<vaccineModels> layTatCa(bool chiLayDangHoatDong = false)
        {
            var sql = @"select maVaccine, tenVaccine, nhomVaccine, doTuoiToiThieu, doTuoiToiDa, donViTuoi, moTa, luuY, trangThai
from Vaccine";
            if (chiLayDangHoatDong)
            {
                sql += " where trangThai = 1";
            }
            sql += " order by maVaccine desc";

            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<vaccineModels>();
            while (doc.Read())
            {
                danhSach.Add(docVaccine(doc));
            }
            return danhSach;
        }

        // Mục đích: phương thức layTheoMa thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public vaccineModels? layTheoMa(int maVaccine)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"select maVaccine, tenVaccine, nhomVaccine, doTuoiToiThieu, doTuoiToiDa, donViTuoi, moTa, luuY, trangThai
from Vaccine where maVaccine = @maVaccine";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@maVaccine", maVaccine);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            // ExecuteReader dùng để đọc từng dòng dữ liệu trả về từ câu SELECT.
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? docVaccine(doc) : null;
        }

        // Mục đích: phương thức them thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void them(vaccineModels vaccine)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"insert into Vaccine(tenVaccine, nhomVaccine, doTuoiToiThieu, doTuoiToiDa, donViTuoi, moTa, luuY, trangThai)
values(@tenVaccine, @nhomVaccine, @doTuoiToiThieu, @doTuoiToiDa, @donViTuoi, @moTa, @luuY, @trangThai)";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, vaccine);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức capNhat thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        public void capNhat(vaccineModels vaccine)
        {
            // Câu lệnh SQL này dùng để lấy, thêm, sửa hoặc xóa dữ liệu theo đúng nghiệp vụ của phương thức hiện tại.
            // Các giá trị động được truyền bằng tham số @... để tránh ghép chuỗi trực tiếp, giúp truy vấn rõ ràng và an toàn hơn.
            const string sql = @"update Vaccine
set tenVaccine = @tenVaccine, nhomVaccine = @nhomVaccine, doTuoiToiThieu = @doTuoiToiThieu,
    doTuoiToiDa = @doTuoiToiDa, donViTuoi = @donViTuoi, moTa = @moTa, luuY = @luuY, trangThai = @trangThai
where maVaccine = @maVaccine";
            using var ketNoi = coSoDuLieu.taoKetNoi();
            // Tạo SqlCommand để gắn câu SQL với kết nối, sau đó truyền tham số trước khi thực thi.
            using var lenh = new SqlCommand(sql, ketNoi);
            ganThamSo(lenh, vaccine);
            lenh.Parameters.AddWithValue("@maVaccine", vaccine.maVaccine);
            // Mở kết nối ngay trước khi thực thi để giảm thời gian giữ kết nối database.
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // Mục đích: phương thức ganThamSo thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static void ganThamSo(SqlCommand lenh, vaccineModels vaccine)
        {
            lenh.Parameters.AddWithValue("@tenVaccine", vaccine.tenVaccine);
            lenh.Parameters.AddWithValue("@nhomVaccine", (object?)vaccine.nhomVaccine ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiThieu", (object?)vaccine.doTuoiToiThieu ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@doTuoiToiDa", (object?)vaccine.doTuoiToiDa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@donViTuoi", (object?)vaccine.donViTuoi ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@moTa", (object?)vaccine.moTa ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@luuY", (object?)vaccine.luuY ?? DBNull.Value);
            lenh.Parameters.AddWithValue("@trangThai", vaccine.trangThai);
        }

        // Mục đích: phương thức docVaccine thực hiện thao tác đọc/ghi dữ liệu trong SQL Server cho chức năng tương ứng.
        // Dữ liệu đầu vào: các tham số nghiệp vụ hoặc model được Controller truyền xuống để tạo câu lệnh SQL và tham số SQL.
        // Xử lý chính: tạo SqlConnection, tạo SqlCommand, gán tham số chống lỗi SQL injection, mở kết nối và thực thi câu lệnh.
        // Kết quả trả về: dữ liệu model/danh sách/giá trị kiểm tra hoặc true/false cho biết thao tác database có thành công hay không.
        private static vaccineModels docVaccine(SqlDataReader doc)
        {
            return new vaccineModels
            {
                maVaccine = Convert.ToInt32(doc["maVaccine"]),
                tenVaccine = doc["tenVaccine"].ToString() ?? string.Empty,
                nhomVaccine = doc["nhomVaccine"] == DBNull.Value ? null : doc["nhomVaccine"].ToString(),
                doTuoiToiThieu = doc["doTuoiToiThieu"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiThieu"]),
                doTuoiToiDa = doc["doTuoiToiDa"] == DBNull.Value ? null : Convert.ToInt32(doc["doTuoiToiDa"]),
                donViTuoi = doc["donViTuoi"] == DBNull.Value ? null : doc["donViTuoi"].ToString(),
                moTa = doc["moTa"] == DBNull.Value ? null : doc["moTa"].ToString(),
                luuY = doc["luuY"] == DBNull.Value ? null : doc["luuY"].ToString(),
                trangThai = Convert.ToBoolean(doc["trangThai"])
            };
        }
    }
}
