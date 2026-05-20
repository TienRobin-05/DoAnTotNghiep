using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class vaccineModels
    {
        public int maVaccine { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên vaccine")]
        public string tenVaccine { get; set; } = string.Empty;

        public string? nhomVaccine { get; set; }
        public int? doTuoiToiThieu { get; set; }
        public int? doTuoiToiDa { get; set; }
        public string? donViTuoi { get; set; }
        public string? moTa { get; set; }
        public string? luuY { get; set; }
        public bool trangThai { get; set; } = true;
    }
}
