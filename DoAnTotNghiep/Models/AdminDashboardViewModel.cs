namespace DoAnTotNghiep.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalVaccines { get; set; }
        public int TotalInjectionDoses { get; set; }
        public int TotalGuideArticles { get; set; }
        public int PendingQuestions { get; set; }
        public int NewUsers7Days { get; set; }
        public int NewArticles7Days { get; set; }
        public int UpcomingAppointments { get; set; }
        public int ActiveVaccines { get; set; }
        public int AnsweredQuestions7Days { get; set; }
        public int RecordedInjections { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public List<Vaccine> RecentVaccines { get; set; } = new();
        public List<CauHoiTuVan> RecentQuestions { get; set; } = new();
        public List<BaiVietCamNang> LatestGuideArticles { get; set; } = new();
    }
}
