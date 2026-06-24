using System.Net;
using System.Text.Json;
using DoAnTotNghiep.DAL;
using WebPush;

namespace DoAnTotNghiep.Services
{
    public class PushNotificationService
    {
        private readonly PushSubscription_DAL pushSubscriptionDAL;
        private readonly IConfiguration configuration;
        private readonly TaiKhoan_DAL taiKhoanDAL;

        public PushNotificationService(PushSubscription_DAL pushSubscriptionDAL, IConfiguration configuration, TaiKhoan_DAL taiKhoanDAL)
        {
            this.pushSubscriptionDAL = pushSubscriptionDAL;
            this.configuration = configuration;
            this.taiKhoanDAL = taiKhoanDAL;
        }

        public string LayPublicKey()
        {
            return configuration["WebPush:PublicKey"] ?? string.Empty;
        }

        public void GuiThongBao(int maTaiKhoan, int maThongBao, string tieuDe, string noiDung)
        {
            var publicKey = configuration["WebPush:PublicKey"] ?? string.Empty;
            var privateKey = configuration["WebPush:PrivateKey"] ?? string.Empty;
            var subject = configuration["WebPush:Subject"] ?? "mailto:admin@doantotnghiep.local";

            if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey))
            {
                return;
            }

            // Kiểm tra user đã bật push chưa
            bool pushEnabled = taiKhoanDAL.GetPushNotificationEnabled(maTaiKhoan);
            if (!pushEnabled)
            {
                System.Console.WriteLine($"[PushSend] User {maTaiKhoan} da tat push notification. Bo qua.");
                return;
            }

            var payload = JsonSerializer.Serialize(new
            {
                title = tieuDe,
                body = noiDung,
                url = $"/ThongBao/Details/{maThongBao}",
                icon = "/favicon.ico"
            });

            var vapid = new VapidDetails(subject, publicKey, privateKey);
            var client = new WebPushClient();
            var danhSachDangKy = pushSubscriptionDAL.LayTheoTaiKhoan(maTaiKhoan);

            System.Console.WriteLine($"[PushSend] UserId={maTaiKhoan}, enabled={pushEnabled}, subscriptions={danhSachDangKy.Count}");

            foreach (var dangKy in danhSachDangKy)
            {
                var subscription = new WebPush.PushSubscription(dangKy.Endpoint, dangKy.P256dh, dangKy.Auth);

                try
                {
                    client.SendNotificationAsync(subscription, payload, vapid).GetAwaiter().GetResult();
                    System.Console.WriteLine($"[PushSend] Da gui push thanh cong toi endpoint: {dangKy.Endpoint?.Substring(0, 50)}...");
                }
                catch (WebPushException ex) when (ex.StatusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.NotFound)
                {
                    System.Console.WriteLine($"[PushSend] Xoa endpoint khong con hieu luc: {dangKy.Endpoint?.Substring(0, 50)}...");
                    pushSubscriptionDAL.XoaTheoEndpoint(dangKy.Endpoint);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"[PushSend] Loi gui push: {ex.Message}");
                }
            }
        }
    }
}
