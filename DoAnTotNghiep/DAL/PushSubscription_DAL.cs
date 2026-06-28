using System.Security.Cryptography;
using System.Text;
using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class PushSubscription_DAL
    {
        private readonly string chuoiKetNoi;

        public PushSubscription_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        // khởi tạo bảng nếu chưa có
        public void KhoiTaoBangNeuChuaCo()
        {
            const string sql = @"
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID(N'dbo.PushSubscription', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PushSubscription (
        maPushSubscription INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PushSubscription PRIMARY KEY,
        maTaiKhoan INT NOT NULL,
        endpoint NVARCHAR(MAX) NOT NULL,
        endpointHash NVARCHAR(64) NOT NULL,
        p256dh NVARCHAR(MAX) NOT NULL,
        auth NVARCHAR(MAX) NOT NULL,
        ngayTao DATETIME NOT NULL CONSTRAINT DF_PushSubscription_ngayTao DEFAULT GETDATE(),
        ngayCapNhat DATETIME NULL,
        isActive BIT NOT NULL CONSTRAINT DF_PushSubscription_isActive DEFAULT 1
    );
END;

IF COL_LENGTH(N'dbo.PushSubscription', N'endpointHash') IS NULL
BEGIN
    ALTER TABLE dbo.PushSubscription ADD endpointHash NVARCHAR(64) NULL;
END;

IF COL_LENGTH(N'dbo.PushSubscription', N'isActive') IS NULL
BEGIN
    ALTER TABLE dbo.PushSubscription
    ADD isActive BIT NOT NULL CONSTRAINT DF_PushSubscription_isActive DEFAULT 1;
END;

IF COL_LENGTH(N'dbo.PushSubscription', N'ngayCapNhat') IS NULL
BEGIN
    ALTER TABLE dbo.PushSubscription ADD ngayCapNhat DATETIME NULL;
END;

UPDATE dbo.PushSubscription
SET endpointHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARBINARY(MAX), endpoint)), 2)
WHERE endpointHash IS NULL
AND endpoint IS NOT NULL;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_PushSubscription_endpointHash'
    AND object_id = OBJECT_ID(N'dbo.PushSubscription')
)
BEGIN
    CREATE UNIQUE INDEX UX_PushSubscription_endpointHash
    ON dbo.PushSubscription(endpointHash)
    WHERE endpointHash IS NOT NULL;
END;

IF OBJECT_ID(N'dbo.TaiKhoan', N'U') IS NOT NULL
AND COL_LENGTH(N'dbo.TaiKhoan', N'maTaiKhoan') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_PushSubscription_TaiKhoan'
    AND parent_object_id = OBJECT_ID(N'dbo.PushSubscription')
)
BEGIN
    ALTER TABLE dbo.PushSubscription
    ADD CONSTRAINT FK_PushSubscription_TaiKhoan
    FOREIGN KEY (maTaiKhoan) REFERENCES dbo.TaiKhoan(maTaiKhoan);
END;";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);

            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        // lưu đăng ký push
        public void LuuDangKy(int maTaiKhoan, PushSubscriptionRequest request)
        {
            const string sql = @"IF EXISTS (SELECT 1 FROM PushSubscription WHERE endpointHash = @EndpointHash OR endpoint = @Endpoint)
BEGIN
    UPDATE PushSubscription
    SET maTaiKhoan = @MaTaiKhoan,
        endpoint = @Endpoint,
        endpointHash = @EndpointHash,
        p256dh = @P256dh,
        auth = @Auth,
        ngayCapNhat = GETDATE(),
        isActive = 1
    WHERE endpointHash = @EndpointHash
    OR endpoint = @Endpoint
END
ELSE
BEGIN
    INSERT INTO PushSubscription(maTaiKhoan, endpoint, endpointHash, p256dh, auth, ngayTao, ngayCapNhat, isActive)
    VALUES(@MaTaiKhoan, @Endpoint, @EndpointHash, @P256dh, @Auth, GETDATE(), GETDATE(), 1)
END";

            ThucThiVoiKhoiTaoLaiNeuThieuBang(lenh =>
            {
                lenh.CommandText = sql;
                GanThamSoDangKy(lenh, maTaiKhoan, request);
                lenh.ExecuteNonQuery();
            });
        }

        // lấy danh sách đăng ký push
        public List<PushSubscriptionModel> LayTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"SELECT maPushSubscription, maTaiKhoan, endpoint, p256dh, auth
FROM PushSubscription
WHERE maTaiKhoan = @MaTaiKhoan
AND ISNULL(isActive, 1) = 1";

            try
            {
                using var ketNoi = new SqlConnection(chuoiKetNoi);
                using var lenh = new SqlCommand(sql, ketNoi);
                lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                ketNoi.Open();
                using var doc = lenh.ExecuteReader();

                var danhSach = new List<PushSubscriptionModel>();
                while (doc.Read())
                {
                    danhSach.Add(new PushSubscriptionModel
                    {
                        MaPushSubscription = Convert.ToInt32(doc["maPushSubscription"]),
                        MaTaiKhoan = Convert.ToInt32(doc["maTaiKhoan"]),
                        Endpoint = doc["endpoint"].ToString() ?? string.Empty,
                        P256dh = doc["p256dh"].ToString() ?? string.Empty,
                        Auth = doc["auth"].ToString() ?? string.Empty
                    });
                }

                return danhSach;
            }
            catch (SqlException ex) when (LaLoiThieuBangPushSubscription(ex))
            {
                KhoiTaoBangNeuChuaCo();
                return new List<PushSubscriptionModel>();
            }
        }

        // xóa đăng ký push theo endpoint
        public void XoaTheoEndpoint(string endpoint)
        {
            const string sql = @"UPDATE PushSubscription
SET isActive = 0,
    ngayCapNhat = GETDATE()
WHERE endpointHash = @EndpointHash
OR endpoint = @Endpoint";

            ThucThiVoiKhoiTaoLaiNeuThieuBang(lenh =>
            {
                lenh.CommandText = sql;
                lenh.Parameters.AddWithValue("@EndpointHash", TaoEndpointHash(endpoint));
                lenh.Parameters.AddWithValue("@Endpoint", endpoint);
                lenh.ExecuteNonQuery();
            });
        }

        // thực thi, tự tạo bảng nếu thiếu
        private void ThucThiVoiKhoiTaoLaiNeuThieuBang(Action<SqlCommand> thaoTac)
        {
            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand();
            lenh.Connection = ketNoi;

            ketNoi.Open();
            try
            {
                thaoTac(lenh);
            }
            catch (SqlException ex) when (LaLoiThieuBangPushSubscription(ex))
            {
                KhoiTaoBangNeuChuaCo();
                lenh.Parameters.Clear();
                thaoTac(lenh);
            }
        }

        // gán tham số đăng ký
        private static void GanThamSoDangKy(SqlCommand lenh, int maTaiKhoan, PushSubscriptionRequest request)
        {
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@Endpoint", request.Endpoint);
            lenh.Parameters.AddWithValue("@EndpointHash", TaoEndpointHash(request.Endpoint));
            lenh.Parameters.AddWithValue("@P256dh", request.Keys.P256dh);
            lenh.Parameters.AddWithValue("@Auth", request.Keys.Auth);
        }

        // kiểm tra lỗi thiếu bảng
        private static bool LaLoiThieuBangPushSubscription(SqlException ex)
        {
            foreach (SqlError error in ex.Errors)
            {
                if (error.Number == 208 && error.Message.Contains("PushSubscription", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // tạo hash cho endpoint
        private static string TaoEndpointHash(string endpoint)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(endpoint));
            return Convert.ToHexString(bytes);
        }
    }
}
