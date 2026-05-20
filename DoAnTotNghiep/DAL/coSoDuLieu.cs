using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class coSoDuLieu
    {
        private readonly string chuoiKetNoi;

        public coSoDuLieu(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public SqlConnection taoKetNoi()
        {
            return new SqlConnection(chuoiKetNoi);
        }
    }
}
