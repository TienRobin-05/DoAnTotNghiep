using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.Models
{
    public class Thong_Tin_Ca_Nhan_View_Model
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string hoTen { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public string gioiTinh { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? ngaySinh { get; set; }

        public string soDienThoai { get; set; } = "";
    }
}
