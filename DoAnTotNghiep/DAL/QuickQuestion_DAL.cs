using DoAnTotNghiep.Models;
using Microsoft.Data.SqlClient;

namespace DoAnTotNghiep.DAL
{
    public class QuickQuestion_DAL
    {
        private readonly string chuoiKetNoi;

        public QuickQuestion_DAL(IConfiguration configuration)
        {
            chuoiKetNoi = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public void KhoiTaoBangNeuChuaCo()
        {
            const string sql = @"
IF OBJECT_ID('dbo.QuickQuestion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.QuickQuestion
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        question NVARCHAR(500) NOT NULL,
        content NVARCHAR(MAX) NOT NULL,
        topic NVARCHAR(150) NOT NULL,
        badgeLabel NVARCHAR(150) NULL,
        sortOrder INT NOT NULL CONSTRAINT DF_QuickQuestion_sortOrder DEFAULT 0,
        isActive BIT NOT NULL CONSTRAINT DF_QuickQuestion_isActive DEFAULT 1,
        createdBy INT NULL,
        updatedBy INT NULL,
        createdAt DATETIME2 NOT NULL CONSTRAINT DF_QuickQuestion_createdAt DEFAULT SYSUTCDATETIME(),
        updatedAt DATETIME2 NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.QuickQuestion)
BEGIN
    INSERT INTO dbo.QuickQuestion (question, content, topic, badgeLabel, sortOrder, isActive)
    VALUES
    (N'Sau khi tiêm vaccine bị sốt nhẹ có sao không?', N'Tôi muốn hỏi sau khi tiêm vaccine bị sốt nhẹ thì có nguy hiểm không và cần theo dõi như thế nào?', N'Phản ứng sau tiêm', N'Phản ứng sau tiêm', 1, 1),
    (N'Quên lịch tiêm thì có cần tiêm lại từ đầu không?', N'Tôi bị quên lịch tiêm theo hẹn. Tôi muốn hỏi có cần tiêm lại từ đầu hay chỉ cần tiêm tiếp mũi còn thiếu?', N'Lịch tiêm', N'Lịch tiêm', 2, 1),
    (N'Phụ nữ mang thai có tiêm vaccine cúm được không?', N'Tôi muốn hỏi phụ nữ đang mang thai có thể tiêm vaccine cúm được không và thời điểm nào là phù hợp?', N'Vaccine cúm', N'Vaccine cúm', 3, 1),
    (N'Tiêm HPV cần mấy mũi?', N'Tôi muốn hỏi vaccine HPV cần tiêm mấy mũi và khoảng cách giữa các mũi là bao lâu?', N'HPV', N'HPV', 4, 1),
    (N'Có thể tiêm nhiều vaccine trong cùng một ngày không?', N'Tôi muốn hỏi có thể tiêm nhiều loại vaccine trong cùng một ngày không và cần lưu ý gì trước khi tiêm?', N'Lịch tiêm', N'Lịch tiêm', 5, 1);
END;";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            ketNoi.Open();
            lenh.ExecuteNonQuery();
        }

        public List<QuickQuestion> LayDanhSach(string? keyword = null, string? topic = null, bool? isActive = null)
        {
            KhoiTaoBangNeuChuaCo();
            const string sql = @"SELECT id, question, content, topic, badgeLabel, sortOrder, isActive, createdBy, updatedBy, createdAt, updatedAt
FROM dbo.QuickQuestion
WHERE (@Keyword IS NULL OR question LIKE @KeywordLike OR content LIKE @KeywordLike OR topic LIKE @KeywordLike)
AND (@Topic IS NULL OR topic = @Topic)
AND (@IsActive IS NULL OR isActive = @IsActive)
ORDER BY sortOrder ASC, id ASC";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Keyword", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim());
            lenh.Parameters.AddWithValue("@KeywordLike", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : $"%{keyword.Trim()}%");
            lenh.Parameters.AddWithValue("@Topic", string.IsNullOrWhiteSpace(topic) ? DBNull.Value : topic.Trim());
            lenh.Parameters.AddWithValue("@IsActive", isActive.HasValue ? isActive.Value : DBNull.Value);

            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            var danhSach = new List<QuickQuestion>();
            while (doc.Read())
            {
                danhSach.Add(DocQuickQuestion(doc));
            }

            return danhSach;
        }

        public List<QuickQuestion> LayDanhSachDangBat(string? keyword = null)
        {
            return LayDanhSach(keyword, null, true);
        }

        public QuickQuestion? LayTheoId(int id)
        {
            KhoiTaoBangNeuChuaCo();
            const string sql = @"SELECT id, question, content, topic, badgeLabel, sortOrder, isActive, createdBy, updatedBy, createdAt, updatedAt
FROM dbo.QuickQuestion
WHERE id = @Id";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Id", id);
            ketNoi.Open();
            using var doc = lenh.ExecuteReader();
            return doc.Read() ? DocQuickQuestion(doc) : null;
        }

        public bool Them(QuickQuestion quickQuestion, int? maAdmin)
        {
            KhoiTaoBangNeuChuaCo();
            const string sql = @"INSERT INTO dbo.QuickQuestion
(question, content, topic, badgeLabel, sortOrder, isActive, createdBy, createdAt)
VALUES
(@Question, @Content, @Topic, @BadgeLabel, @SortOrder, @IsActive, @CreatedBy, SYSUTCDATETIME())";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            GanThamSo(lenh, quickQuestion);
            lenh.Parameters.AddWithValue("@CreatedBy", (object?)maAdmin ?? DBNull.Value);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool CapNhat(QuickQuestion quickQuestion, int? maAdmin)
        {
            KhoiTaoBangNeuChuaCo();
            const string sql = @"UPDATE dbo.QuickQuestion
SET question = @Question,
    content = @Content,
    topic = @Topic,
    badgeLabel = @BadgeLabel,
    sortOrder = @SortOrder,
    isActive = @IsActive,
    updatedBy = @UpdatedBy,
    updatedAt = SYSUTCDATETIME()
WHERE id = @Id";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Id", quickQuestion.Id);
            GanThamSo(lenh, quickQuestion);
            lenh.Parameters.AddWithValue("@UpdatedBy", (object?)maAdmin ?? DBNull.Value);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool CapNhatTrangThai(int id, bool isActive, int? maAdmin)
        {
            KhoiTaoBangNeuChuaCo();
            const string sql = @"UPDATE dbo.QuickQuestion
SET isActive = @IsActive,
    updatedBy = @UpdatedBy,
    updatedAt = SYSUTCDATETIME()
WHERE id = @Id";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Id", id);
            lenh.Parameters.AddWithValue("@IsActive", isActive);
            lenh.Parameters.AddWithValue("@UpdatedBy", (object?)maAdmin ?? DBNull.Value);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int id)
        {
            KhoiTaoBangNeuChuaCo();
            const string sql = "DELETE FROM dbo.QuickQuestion WHERE id = @Id";

            using var ketNoi = new SqlConnection(chuoiKetNoi);
            using var lenh = new SqlCommand(sql, ketNoi);
            lenh.Parameters.AddWithValue("@Id", id);
            ketNoi.Open();
            return lenh.ExecuteNonQuery() > 0;
        }

        private static void GanThamSo(SqlCommand lenh, QuickQuestion quickQuestion)
        {
            lenh.Parameters.AddWithValue("@Question", quickQuestion.Question.Trim());
            lenh.Parameters.AddWithValue("@Content", quickQuestion.Content.Trim());
            lenh.Parameters.AddWithValue("@Topic", quickQuestion.Topic.Trim());
            lenh.Parameters.AddWithValue("@BadgeLabel", string.IsNullOrWhiteSpace(quickQuestion.BadgeLabel) ? quickQuestion.Topic.Trim() : quickQuestion.BadgeLabel.Trim());
            lenh.Parameters.AddWithValue("@SortOrder", quickQuestion.SortOrder);
            lenh.Parameters.AddWithValue("@IsActive", quickQuestion.IsActive);
        }

        private static QuickQuestion DocQuickQuestion(SqlDataReader doc)
        {
            return new QuickQuestion
            {
                Id = Convert.ToInt32(doc["id"]),
                Question = doc["question"].ToString() ?? string.Empty,
                Content = doc["content"].ToString() ?? string.Empty,
                Topic = doc["topic"].ToString() ?? string.Empty,
                BadgeLabel = doc["badgeLabel"] == DBNull.Value ? string.Empty : doc["badgeLabel"].ToString() ?? string.Empty,
                SortOrder = Convert.ToInt32(doc["sortOrder"]),
                IsActive = Convert.ToBoolean(doc["isActive"]),
                CreatedBy = doc["createdBy"] == DBNull.Value ? null : Convert.ToInt32(doc["createdBy"]),
                UpdatedBy = doc["updatedBy"] == DBNull.Value ? null : Convert.ToInt32(doc["updatedBy"]),
                CreatedAt = Convert.ToDateTime(doc["createdAt"]),
                UpdatedAt = doc["updatedAt"] == DBNull.Value ? null : Convert.ToDateTime(doc["updatedAt"])
            };
        }
    }
}
