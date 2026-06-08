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

        public PushNotificationService(PushSubscription_DAL pushSubscriptionDAL, IConfiguration configuration)
        {
            this.pushSubscriptionDAL = pushSubscriptionDAL;
            this.configuration = configuration;
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

            foreach (var dangKy in danhSachDangKy)
            {
                var subscription = new WebPush.PushSubscription(dangKy.Endpoint, dangKy.P256dh, dangKy.Auth);

                try
                {
                    client.SendNotificationAsync(subscription, payload, vapid).GetAwaiter().GetResult();
                }
                catch (WebPushException ex) when (ex.StatusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.NotFound)
                {
                    pushSubscriptionDAL.XoaTheoEndpoint(dangKy.Endpoint);
                }
                catch
                {
                    // Không làm hỏng luồng tạo thông báo nếu trình duyệt tạm thời không nhận được push.
                }
            }
        }
    }
}
