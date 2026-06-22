namespace DoAnTotNghiep.Models
{
    public class AdminVaccineIndexViewModel
    {
        public int TotalVaccines { get; set; }
        public int ActiveVaccines { get; set; }
        public string LastUpdatedText { get; set; } = string.Empty;
        public string? Keyword { get; set; }
        public string? SelectedGroup { get; set; }
        public string? SelectedStatus { get; set; }
        public List<string> VaccineGroups { get; set; } = new();
        public List<Vaccine> Vaccines { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int StartItem { get; set; }
        public int EndItem { get; set; }
        public int TotalPages { get; set; } = 1;
    }
}
