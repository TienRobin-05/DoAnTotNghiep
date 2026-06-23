namespace DoAnTotNghiep.Models
{
    public class AdminDoseIndexViewModel
    {
        public int TotalDoses { get; set; }
        public string? Keyword { get; set; }
        public string? SelectedIntervalType { get; set; }
        public List<AdminDoseItemViewModel> Items { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int StartItem { get; set; }
        public int EndItem { get; set; }
        public int TotalPages { get; set; }
    }

    public class AdminDoseItemViewModel
    {
        public int Id { get; set; }
        public int VaccineId { get; set; }
        public string VaccineName { get; set; } = "";
        public int DoseNumber { get; set; }
        public string DoseName { get; set; } = "";
        public string RecommendedScheduleText { get; set; } = "";
        public string IntervalText { get; set; } = "";
    }
}
