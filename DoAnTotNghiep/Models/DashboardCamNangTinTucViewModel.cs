namespace DoAnTotNghiep.Models
{
    public class DashboardCamNangTinTucViewModel
    {
        public List<BaiVietCamNang> BaiVietNoiBat { get; set; } = new();
        public List<BaiVietCamNang> BaiVietMoiNhat { get; set; } = new();
    }
}
