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

        public void LuuDangKy(int maTaiKhoan, PushSubscriptionRequest request)
        {
            const string sql = @"IF EXISTS (SELECT 1 FROM PushSubscription WHERE endpointHash = @EndpointHash)
BEGIN
    UPDATE PushSubscription
    SET maTaiKhoan = @MaTaiKhoan,
        endpoint = @Endpoint,
        p256dh = @P256dh,
        auth = @Auth,
        ngayCapNhat = GETDATE()
    WHERE endpointHash = @EndpointHash
END
ELSE
BEGIN
    INSERT INTO PushSubscription(maTaiKhoan, endpoint, endpointHash, p256dh, auth, ngayTao, ngayCapNhat)
    VALUES(@MaTaiKhoan, @Endpoint, @EndpointHash, @P256dh, @Auth, GETDATE(), GETDATE())
END";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);
            lenh.Parameters.AddWithValue("@Endpoint", request.Endpoint);
            lenh.Parameters.AddWithValue("@EndpointHash", TaoEndpointHash(request.Endpoint));
            lenh.Parameters.AddWithValue("@P256dh", request.Keys.P256dh);
            lenh.Parameters.AddWithValue("@Auth", request.Keys.Auth);

            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public List<PushSubscriptionModel> LayTheoTaiKhoan(int maTaiKhoan)
        {
            const string sql = @"SELECT maPushSubscription, maTaiKhoan, endpoint, p256dh, auth
FROM PushSubscription
WHERE maTaiKhoan = @MaTaiKhoan";

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

        public void XoaTheoEndpoint(string endpoint)
        {
            const string sql = "DELETE FROM PushSubscription WHERE endpointHash = @EndpointHash";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@EndpointHash", TaoEndpointHash(endpoint));

            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        private static string TaoEndpointHash(string endpoint)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(endpoint));
            return Convert.ToHexString(bytes);
        }
    }
}
