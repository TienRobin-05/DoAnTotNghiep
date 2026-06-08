namespace DoAnTotNghiep.Models
{
    public class PushSubscriptionRequest
    {
        public string Endpoint { get; set; } = string.Empty;
        public PushSubscriptionKeys Keys { get; set; } = new();
    }

    public class PushSubscriptionKeys
    {
        public string P256dh { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
    }
}
