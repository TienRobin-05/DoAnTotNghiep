namespace DoAnTotNghiep.Models
{
    public class LichTiemRoadmapViewModel
    {
        public List<LichTiem> QuaHanList { get; set; } = new();
        public List<LichTiem> GanList { get; set; } = new();
        public List<LichTiem> TrungHanList { get; set; } = new();
        public List<LichTiem> DaiHanList { get; set; } = new();
        public List<LichTiem> DaTiemList { get; set; } = new();
        public List<LichTiem> AllList { get; set; } = new();
    }
}
