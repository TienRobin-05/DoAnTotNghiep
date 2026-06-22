namespace DoAnTotNghiep.Models
{
    public class QuickQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string BadgeLabel { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
