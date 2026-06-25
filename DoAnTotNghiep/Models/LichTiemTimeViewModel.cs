namespace DoAnTotNghiep.Models
{
    public class LichTiemTimeViewModel
    {
        public List<LichTiem> QuaHanList { get; set; } = new();
        public List<TimeScheduleGroup> TimeGroups { get; set; } = new();
        public List<LichTiem> DaTiemList { get; set; } = new();
        public List<LichTiem> AllList { get; set; } = new();
    }

    public class TimeScheduleGroup
    {
        public string Title { get; set; } = "";
        public string Type { get; set; } = "upcoming";
        public int Count { get; set; }
        public List<LichTiem> Items { get; set; } = new();
    }
}
