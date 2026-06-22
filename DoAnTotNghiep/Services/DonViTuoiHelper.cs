using System.Globalization;
using System.Text;

namespace DoAnTotNghiep.Services
{
    public static class DonViTuoiHelper
    {
        public static readonly string[] DonViHopLe = { "ngày", "tuần", "tháng", "năm" };

        public static string ChuanHoa(string? donViTuoi)
        {
            var donVi = (donViTuoi ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(donVi))
            {
                return "ngày";
            }

            var khongDau = BoDauTiengViet(donVi);
            return khongDau switch
            {
                "ngay" => "ngày",
                "tuan" => "tuần",
                "thang" => "tháng",
                "nam" => "năm",
                "tuoi" => "năm",
                _ => donVi
            };
        }

        public static bool HopLe(string? donViTuoi)
        {
            if (string.IsNullOrWhiteSpace(donViTuoi))
            {
                return true;
            }

            var donVi = ChuanHoa(donViTuoi);
            return DonViHopLe.Contains(donVi, StringComparer.OrdinalIgnoreCase);
        }

        public static DateTime? CongTheoDonVi(DateTime ngaySinh, int soLuong, string? donViTuoi)
        {
            try
            {
                return ChuanHoa(donViTuoi) switch
                {
                    "năm" => ngaySinh.AddYears(soLuong),
                    "tháng" => ngaySinh.AddMonths(soLuong),
                    "tuần" => ngaySinh.AddDays(soLuong * 7),
                    "ngày" => ngaySinh.AddDays(soLuong),
                    _ => null
                };
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public static int TinhTuoiHienTai(DateTime ngaySinh, string? donViTuoi)
        {
            var ngayHienTai = DateTime.Today;
            var ngaySinhDate = ngaySinh.Date;
            if (ngaySinhDate > ngayHienTai)
            {
                return 0;
            }

            return ChuanHoa(donViTuoi) switch
            {
                "năm" => TinhSoNamTuoi(ngaySinhDate, ngayHienTai),
                "tháng" => TinhSoThangTuoi(ngaySinhDate, ngayHienTai),
                "tuần" => Math.Max(0, (ngayHienTai - ngaySinhDate).Days / 7),
                _ => Math.Max(0, (ngayHienTai - ngaySinhDate).Days)
            };
        }

        public static string DanhSachDonViHopLe()
        {
            return string.Join(", ", DonViHopLe);
        }

        private static int TinhSoThangTuoi(DateTime ngaySinh, DateTime ngayHienTai)
        {
            var soThang = ((ngayHienTai.Year - ngaySinh.Year) * 12) + ngayHienTai.Month - ngaySinh.Month;
            if (ngayHienTai.Day < ngaySinh.Day)
            {
                soThang--;
            }

            return Math.Max(0, soThang);
        }

        private static int TinhSoNamTuoi(DateTime ngaySinh, DateTime ngayHienTai)
        {
            var soNam = ngayHienTai.Year - ngaySinh.Year;
            if (ngayHienTai.Date < ngaySinh.Date.AddYears(soNam))
            {
                soNam--;
            }

            return Math.Max(0, soNam);
        }

        private static string BoDauTiengViet(string giaTri)
        {
            var normalized = giaTri.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (var kyTu in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(kyTu) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(kyTu);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
