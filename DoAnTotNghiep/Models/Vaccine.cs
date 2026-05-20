namespace DoAnTotNghiep.Models
{
    public class Vaccine
    {
        public int MaVaccine { get; set; }
        public string TenVaccine { get; set; } = string.Empty;
        public string NhomVaccine { get; set; } = string.Empty;
        public int? DoTuoiToiThieu { get; set; }
        public int? DoTuoiToiDa { get; set; }
        public string DonViTuoi { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string LuuY { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
    }
}
