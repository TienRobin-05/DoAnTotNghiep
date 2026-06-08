namespace DoAnTotNghiep.Models
{
    public class PushSubscriptionModel
    {
        public int MaPushSubscription { get; set; }
        public int MaTaiKhoan { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string P256dh { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
    }
}
