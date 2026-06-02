using System.Security.Cryptography;

namespace DoAnTotNghiep.Services
{
    /// <summary>
    /// Helper hash/verify mat khau bang PBKDF2, khong phu thuoc schema ngoai cot matKhau hien co.
    /// </summary>
    public static class MatKhauService
    {
        private const int KichThuocSalt = 16;
        private const int KichThuocHash = 32;
        private const int SoLanLap = 60000;
        private const string TienToHash = "PBKDF2";

        public static string TaoHash(string matKhau)
        {
            var salt = RandomNumberGenerator.GetBytes(KichThuocSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                matKhau,
                salt,
                SoLanLap,
                HashAlgorithmName.SHA256,
                KichThuocHash);

            return $"{TienToHash}${SoLanLap}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool LaHash(string giaTri)
        {
            return !string.IsNullOrWhiteSpace(giaTri)
                && giaTri.StartsWith($"{TienToHash}$", StringComparison.Ordinal);
        }

        public static string ChuanBiLuu(string matKhau)
        {
            return LaHash(matKhau) ? matKhau : TaoHash(matKhau);
        }

        public static bool KiemTra(string matKhauNhap, string matKhauDaLuu)
        {
            if (string.IsNullOrEmpty(matKhauNhap) || string.IsNullOrEmpty(matKhauDaLuu))
            {
                return false;
            }

            if (!LaHash(matKhauDaLuu))
            {
                return matKhauNhap == matKhauDaLuu;
            }

            var parts = matKhauDaLuu.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var soLanLap))
            {
                return false;
            }

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var hashDaLuu = Convert.FromBase64String(parts[3]);
                var hashNhap = Rfc2898DeriveBytes.Pbkdf2(
                    matKhauNhap,
                    salt,
                    soLanLap,
                    HashAlgorithmName.SHA256,
                    hashDaLuu.Length);

                return CryptographicOperations.FixedTimeEquals(hashNhap, hashDaLuu);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
