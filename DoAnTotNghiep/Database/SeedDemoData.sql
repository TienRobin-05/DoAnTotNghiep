/*
    SeedDemoData.sql
    Database target checked from appsettings.json:
    Data Source=.; Initial Catalog=doAnTotNghiep; Integrated Security=True

    Safety note:
    - This script does not DROP or ALTER business table structure.
    - It deletes old demo/business data, resets identities, then inserts fresh demo data.
    - All delete/insert work is wrapped in one transaction with TRY/CATCH rollback.
*/

USE [doAnTotNghiep];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Today date = CONVERT(date, GETDATE());
    DECLARE @PasswordHash nvarchar(510) = N'PBKDF2$60000$AQIDBAUGBwgJCgsMDQ4PEA==$53JMomx+7JYrs8zG7Lvs6T82rnNJ7NXzx1HZBHsmPjc=';

    DECLARE @BeforeCounts table (TenBang sysname, SoLuong int);
    INSERT INTO @BeforeCounts(TenBang, SoLuong)
    SELECT N'LichSuTiem', COUNT(*) FROM dbo.LichSuTiem UNION ALL
    SELECT N'ThongBao', COUNT(*) FROM dbo.ThongBao UNION ALL
    SELECT N'LichTiem', COUNT(*) FROM dbo.LichTiem UNION ALL
    SELECT N'CauHoiTuVan', COUNT(*) FROM dbo.CauHoiTuVan UNION ALL
    SELECT N'BaiVietCamNang', COUNT(*) FROM dbo.BaiVietCamNang UNION ALL
    SELECT N'HoSoSucKhoe', COUNT(*) FROM dbo.HoSoSucKhoe UNION ALL
    SELECT N'MuiTiemVaccine', COUNT(*) FROM dbo.MuiTiemVaccine UNION ALL
    SELECT N'Vaccine', COUNT(*) FROM dbo.Vaccine UNION ALL
    SELECT N'PushSubscription', COUNT(*) FROM dbo.PushSubscription UNION ALL
    SELECT N'TaiKhoan', COUNT(*) FROM dbo.TaiKhoan;

    DELETE FROM dbo.LichSuTiem;
    DELETE FROM dbo.ThongBao;
    DELETE FROM dbo.LichTiem;
    DELETE FROM dbo.CauHoiTuVan;
    DELETE FROM dbo.BaiVietCamNang;
    DELETE FROM dbo.HoSoSucKhoe;
    DELETE FROM dbo.MuiTiemVaccine;
    DELETE FROM dbo.Vaccine;
    DELETE FROM dbo.PushSubscription;
    DELETE FROM dbo.TaiKhoan;

    DBCC CHECKIDENT ('dbo.LichSuTiem', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.ThongBao', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.LichTiem', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.CauHoiTuVan', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.BaiVietCamNang', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.HoSoSucKhoe', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.MuiTiemVaccine', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.Vaccine', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.PushSubscription', RESEED, 0) WITH NO_INFOMSGS;
    DBCC CHECKIDENT ('dbo.TaiKhoan', RESEED, 0) WITH NO_INFOMSGS;

    INSERT INTO dbo.TaiKhoan(hoTen, email, matKhau, soDienThoai, vaiTro, trangThai, ngayTao, LanDangNhapCuoi, DaXoa, NgayXoaMem, LyDoXoa)
    VALUES
    (N'Quản trị hệ thống', N'admin@pharmacycity.demo', @PasswordHash, N'0901000001', N'Admin', 1, DATEADD(day, -90, GETDATE()), GETDATE(), 0, NULL, NULL),
    (N'Nguyễn Minh Anh', N'minhanh.demo@example.com', @PasswordHash, N'0901000002', N'User', 1, DATEADD(day, -80, GETDATE()), DATEADD(day, -1, GETDATE()), 0, NULL, NULL),
    (N'Trần Quốc Bảo', N'quocbao.demo@example.com', @PasswordHash, N'0901000003', N'User', 1, DATEADD(day, -75, GETDATE()), DATEADD(day, -3, GETDATE()), 0, NULL, NULL),
    (N'Lê Thu Hà', N'thuha.demo@example.com', @PasswordHash, N'0901000004', N'User', 1, DATEADD(day, -65, GETDATE()), DATEADD(day, -4, GETDATE()), 0, NULL, NULL),
    (N'Phạm Gia Huy', N'giahuy.demo@example.com', @PasswordHash, N'0901000005', N'User', 1, DATEADD(day, -55, GETDATE()), DATEADD(day, -8, GETDATE()), 0, NULL, NULL),
    (N'Võ Thanh Tâm', N'thanhtam.demo@example.com', @PasswordHash, N'0901000006', N'User', 1, DATEADD(day, -45, GETDATE()), DATEADD(day, -2, GETDATE()), 0, NULL, NULL),
    (N'Đặng Ngọc Lan', N'ngoclan.demo@example.com', @PasswordHash, N'0901000007', N'User', 1, DATEADD(day, -35, GETDATE()), DATEADD(day, -6, GETDATE()), 0, NULL, NULL),
    (N'Hoàng Đức An', N'ducan.demo@example.com', @PasswordHash, N'0901000008', N'User', 1, DATEADD(day, -25, GETDATE()), DATEADD(day, -5, GETDATE()), 0, NULL, NULL);

    INSERT INTO dbo.HoSoSucKhoe(maTaiKhoan, hoTen, ngaySinh, gioiTinh, chieuCao, canNang, tienSuBenh, diUng, ngayTao)
    VALUES
    (2, N'Bé An Nhiên', '2026-03-15', N'Nữ', 58, 5.4, N'Sinh đủ tháng, sức khỏe ổn định', N'Chưa ghi nhận', DATEADD(day, -60, GETDATE())),
    (2, N'Nguyễn Minh Anh', '1994-08-20', N'Nữ', 162, 54, N'Viêm mũi dị ứng theo mùa', N'Dị ứng hải sản nhẹ', DATEADD(day, -59, GETDATE())),
    (3, N'Bé Quốc Khánh', '2024-11-02', N'Nam', 82, 11.5, N'Từng sốt cao co giật khi 10 tháng', N'Chưa ghi nhận', DATEADD(day, -58, GETDATE())),
    (3, N'Trần Quốc Bảo', '1988-04-12', N'Nam', 172, 70, N'Tăng huyết áp nhẹ', N'Dị ứng penicillin', DATEADD(day, -57, GETDATE())),
    (4, N'Lê Bảo Ngọc', '2017-09-05', N'Nữ', 125, 24, N'Viêm phế quản tái phát', N'Chưa ghi nhận', DATEADD(day, -56, GETDATE())),
    (4, N'Lê Thu Hà', '1991-12-25', N'Nữ', 158, 50, N'Không có bệnh nền', N'Chưa ghi nhận', DATEADD(day, -55, GETDATE())),
    (5, N'Phạm Minh Khang', '2011-05-18', N'Nam', 153, 43, N'Hen phế quản nhẹ', N'Dị ứng bụi nhà', DATEADD(day, -54, GETDATE())),
    (5, N'Bà Nguyễn Thị Mai', '1958-07-10', N'Nữ', 154, 56, N'Đái tháo đường type 2', N'Chưa ghi nhận', DATEADD(day, -53, GETDATE())),
    (6, N'Võ Thanh Tâm', '1983-02-14', N'Nam', 168, 67, N'Rối loạn mỡ máu', N'Dị ứng tôm cua', DATEADD(day, -52, GETDATE())),
    (6, N'Võ Hải Đăng', '2020-01-30', N'Nam', 108, 18, N'Viêm da cơ địa', N'Dị ứng trứng nhẹ', DATEADD(day, -51, GETDATE())),
    (7, N'Đặng Ngọc Lan', '1999-06-09', N'Nữ', 160, 49, N'Không có bệnh nền', N'Chưa ghi nhận', DATEADD(day, -50, GETDATE())),
    (8, N'Ông Hoàng Văn Bình', '1949-03-22', N'Nam', 165, 62, N'Bệnh phổi tắc nghẽn mạn tính', N'Dị ứng aspirin', DATEADD(day, -49, GETDATE()));

    INSERT INTO dbo.Vaccine(tenVaccine, nhomVaccine, doTuoiToiThieu, doTuoiToiDa, donViTuoi, moTa, luuY, trangThai)
    VALUES
    (N'Viêm gan B', N'Vaccine trẻ em', 0, 99, N'Tuổi', N'Phòng bệnh viêm gan B và biến chứng xơ gan, ung thư gan.', N'Theo dõi phản ứng tại chỗ tiêm trong 24 giờ đầu.', 1),
    (N'Lao BCG', N'Vaccine sơ sinh', 0, 12, N'Tháng', N'Phòng bệnh lao nặng ở trẻ nhỏ.', N'Thường để lại sẹo nhỏ tại vị trí tiêm.', 1),
    (N'Vaccine 6 trong 1', N'Vaccine phối hợp', 2, 24, N'Tháng', N'Phòng bạch hầu, ho gà, uốn ván, bại liệt, Hib và viêm gan B.', N'Hoãn tiêm khi trẻ sốt cao hoặc bệnh cấp tính.', 1),
    (N'Bại liệt', N'Vaccine trẻ em', 2, 60, N'Tháng', N'Phòng bệnh bại liệt do virus polio.', N'Cần đủ phác đồ để duy trì miễn dịch.', 1),
    (N'Sởi - Quai bị - Rubella', N'Vaccine phối hợp', 9, 99, N'Tuổi', N'Phòng sởi, quai bị và rubella.', N'Không tiêm cho phụ nữ đang mang thai.', 1),
    (N'Viêm não Nhật Bản', N'Vaccine trẻ em', 1, 15, N'Tuổi', N'Phòng viêm não Nhật Bản, bệnh có nguy cơ di chứng thần kinh.', N'Cần tiêm nhắc theo khuyến cáo.', 1),
    (N'Cúm mùa', N'Vaccine hằng năm', 6, 99, N'Tuổi', N'Giảm nguy cơ mắc cúm và biến chứng nặng.', N'Nên tiêm nhắc lại mỗi năm.', 1),
    (N'HPV', N'Vaccine thanh thiếu niên', 9, 45, N'Tuổi', N'Phòng các bệnh liên quan virus HPV.', N'Hiệu quả tốt nhất khi tiêm trước khi có nguy cơ phơi nhiễm.', 1),
    (N'Thủy đậu', N'Vaccine trẻ em và người lớn', 1, 99, N'Tuổi', N'Phòng bệnh thủy đậu và biến chứng.', N'Tránh mang thai trong thời gian khuyến cáo sau tiêm.', 1),
    (N'Phế cầu', N'Vaccine trẻ em và người cao tuổi', 2, 99, N'Tuổi', N'Phòng bệnh do phế cầu như viêm phổi, viêm màng não.', N'Người bệnh nền nên hỏi ý kiến bác sĩ trước khi tiêm.', 1),
    (N'Rota', N'Vaccine đường uống', 6, 32, N'Tuần', N'Phòng tiêu chảy cấp do Rotavirus.', N'Cần uống đúng khoảng tuổi khuyến cáo.', 1),
    (N'Covid-19', N'Vaccine người lớn', 6, 99, N'Tuổi', N'Giảm nguy cơ bệnh nặng do Covid-19.', N'Tiêm nhắc theo khuyến cáo hiện hành.', 1);

    INSERT INTO dbo.MuiTiemVaccine(maVaccine, soMui, tenMui, doTuoiToiThieu, doTuoiToiDa, doTuoiKhuyenNghi, donViTuoi, khoangCachNgay, ghiChu)
    VALUES
    (1, 1, N'Mũi sơ sinh', 0, 24, 0, N'Ngày', NULL, N'Tiêm trong 24 giờ đầu sau sinh nếu đủ điều kiện.'),
    (1, 2, N'Mũi 2', 1, 2, 2, N'Tháng', 60, N'Tiêm nhắc trong phác đồ cơ bản.'),
    (1, 3, N'Mũi 3', 6, 18, 6, N'Tháng', 120, N'Hoàn tất miễn dịch cơ bản.'),
    (2, 1, N'Mũi BCG', 0, 12, 0, N'Tháng', NULL, N'Nên tiêm sớm trong giai đoạn sơ sinh.'),
    (3, 1, N'Mũi 1', 2, 4, 2, N'Tháng', NULL, N'Mũi đầu của vaccine phối hợp.'),
    (3, 2, N'Mũi 2', 3, 5, 3, N'Tháng', 30, N'Cách mũi trước tối thiểu 1 tháng.'),
    (3, 3, N'Mũi 3', 4, 6, 4, N'Tháng', 30, N'Cách mũi trước tối thiểu 1 tháng.'),
    (3, 4, N'Mũi nhắc', 16, 24, 18, N'Tháng', 420, N'Tiêm nhắc khi trẻ 16-24 tháng.'),
    (4, 1, N'Mũi 1', 2, 4, 2, N'Tháng', NULL, N'Mũi bại liệt cơ bản.'),
    (4, 2, N'Mũi 2', 3, 5, 3, N'Tháng', 30, N'Tiêm cách mũi 1 ít nhất 1 tháng.'),
    (4, 3, N'Mũi 3', 4, 6, 4, N'Tháng', 30, N'Hoàn tất chuỗi cơ bản.'),
    (5, 1, N'Mũi 1', 9, 15, 9, N'Tháng', NULL, N'Mũi đầu MMR.'),
    (5, 2, N'Mũi 2', 18, 72, 18, N'Tháng', 270, N'Tiêm nhắc để tăng miễn dịch.'),
    (6, 1, N'Mũi 1', 12, 36, 12, N'Tháng', NULL, N'Mũi đầu viêm não Nhật Bản.'),
    (6, 2, N'Mũi 2', 12, 36, 13, N'Tháng', 7, N'Cách mũi 1 từ 1-2 tuần.'),
    (6, 3, N'Mũi 3', 24, 60, 24, N'Tháng', 365, N'Tiêm nhắc sau 1 năm.'),
    (7, 1, N'Mũi cúm năm nay', 6, 99, 6, N'Tuổi', NULL, N'Tiêm cúm mùa hằng năm.'),
    (8, 1, N'Mũi 1', 9, 45, 9, N'Tuổi', NULL, N'Mũi đầu HPV.'),
    (8, 2, N'Mũi 2', 9, 45, 9, N'Tuổi', 60, N'Cách mũi 1 khoảng 2 tháng.'),
    (8, 3, N'Mũi 3', 9, 45, 10, N'Tuổi', 180, N'Cách mũi 1 khoảng 6 tháng.'),
    (9, 1, N'Mũi 1', 12, 36, 12, N'Tháng', NULL, N'Mũi đầu thủy đậu.'),
    (9, 2, N'Mũi 2', 48, 72, 48, N'Tháng', 90, N'Tiêm nhắc theo khuyến cáo.'),
    (10, 1, N'Mũi 1', 2, 12, 2, N'Tháng', NULL, N'Mũi đầu phế cầu.'),
    (10, 2, N'Mũi 2', 4, 18, 4, N'Tháng', 60, N'Cách mũi trước tối thiểu 2 tháng.'),
    (10, 3, N'Mũi 3', 6, 24, 6, N'Tháng', 60, N'Hoàn tất phác đồ cơ bản.'),
    (11, 1, N'Liều 1', 6, 14, 6, N'Tuần', NULL, N'Uống liều đầu đúng khoảng tuổi.'),
    (11, 2, N'Liều 2', 10, 24, 10, N'Tuần', 28, N'Uống cách liều 1 tối thiểu 4 tuần.'),
    (11, 3, N'Liều 3', 14, 32, 14, N'Tuần', 28, N'Tùy loại vaccine Rota sử dụng.'),
    (12, 1, N'Mũi cơ bản', 6, 99, 6, N'Tuổi', NULL, N'Tiêm theo nhóm tuổi và nguy cơ.'),
    (12, 2, N'Mũi nhắc', 6, 99, 12, N'Tuổi', 180, N'Tiêm nhắc theo khuyến cáo.');

    ;WITH N AS (
        SELECT TOP (75) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects
    )
    INSERT INTO dbo.LichTiem(maHoSo, maMuiTiem, ngayTiemDuKien, trangThai, ghiChu)
    SELECT
        ((rn - 1) % 12) + 1 AS maHoSo,
        ((rn - 1) % 30) + 1 AS maMuiTiem,
        CASE
            WHEN base.NgayGoiY < hs.ngaySinh THEN DATEADD(day, 30, hs.ngaySinh)
            ELSE base.NgayGoiY
        END AS ngayTiemDuKien,
        CASE
            WHEN rn <= 25 THEN N'Đã tiêm'
            WHEN rn <= 40 THEN N'Quá hạn'
            WHEN rn <= 55 THEN N'Sắp đến hạn'
            WHEN rn <= 65 THEN N'Chưa tiêm'
            ELSE N'Cần tư vấn'
        END AS trangThai,
        CASE
            WHEN rn <= 25 THEN N'Lịch đã hoàn tất, dùng để demo lịch sử tiêm.'
            WHEN rn <= 40 THEN N'Lịch đã quá ngày dự kiến, cần cập nhật trạng thái.'
            WHEN rn <= 55 THEN N'Lịch sắp đến hạn, cần nhắc người dùng.'
            WHEN rn <= 65 THEN N'Lịch dự kiến trong tương lai.'
            ELSE N'Hồ sơ có yếu tố cần hỏi ý kiến bác sĩ trước khi tiêm.'
        END AS ghiChu
    FROM N
    JOIN dbo.HoSoSucKhoe hs ON hs.maHoSo = ((rn - 1) % 12) + 1
    CROSS APPLY (
        SELECT CASE
            WHEN rn <= 25 THEN DATEADD(day, -(30 + (rn % 12) * 10), @Today)
            WHEN rn <= 40 THEN DATEADD(day, -(1 + (rn % 20)), @Today)
            WHEN rn <= 55 THEN DATEADD(day, 1 + (rn % 20), @Today)
            WHEN rn <= 65 THEN DATEADD(day, 45 + (rn % 30), @Today)
            ELSE DATEADD(day, 10 + (rn % 18), @Today)
        END AS NgayGoiY
    ) base
    ORDER BY rn;

    INSERT INTO dbo.LichSuTiem(maLichTiem, ngayTiemThucTe, ghiChu, ngayCapNhat)
    SELECT TOP (25)
        lt.maLichTiem,
        CASE
            WHEN ROW_NUMBER() OVER (ORDER BY lt.maLichTiem) % 5 = 0 THEN DATEADD(day, 14, lt.ngayTiemDuKien)
            WHEN ROW_NUMBER() OVER (ORDER BY lt.maLichTiem) % 3 = 0 THEN DATEADD(day, 3, lt.ngayTiemDuKien)
            ELSE lt.ngayTiemDuKien
        END,
        CASE
            WHEN ROW_NUMBER() OVER (ORDER BY lt.maLichTiem) % 5 = 0 THEN N'Tiêm trễ nhiều ngày, cần điều chỉnh các mũi sau.'
            WHEN ROW_NUMBER() OVER (ORDER BY lt.maLichTiem) % 3 = 0 THEN N'Tiêm trễ vài ngày.'
            ELSE N'Tiêm đúng lịch.'
        END,
        GETDATE()
    FROM dbo.LichTiem lt
    JOIN dbo.HoSoSucKhoe hs ON hs.maHoSo = lt.maHoSo
    WHERE lt.trangThai = N'Đã tiêm'
      AND lt.ngayTiemDuKien >= hs.ngaySinh
      AND lt.ngayTiemDuKien <= @Today
    ORDER BY lt.maLichTiem;

    INSERT INTO dbo.ThongBao(maTaiKhoan, maLichTiem, tieuDe, noiDung, ngayGui, daDoc)
    SELECT TOP (35)
        hs.maTaiKhoan,
        lt.maLichTiem,
        CASE
            WHEN lt.trangThai = N'Quá hạn' THEN N'Quá hạn lịch tiêm'
            WHEN lt.trangThai = N'Cần tư vấn' THEN N'Cần tư vấn trước tiêm'
            WHEN lt.trangThai = N'Đã tiêm' THEN N'Đã cập nhật lịch tiêm'
            ELSE N'Sắp đến lịch tiêm'
        END,
        N'Lịch tiêm của ' + hs.hoTen + N' cho ' + vc.tenVaccine + N' - ' + ISNULL(mt.tenMui, N'Mũi tiêm') + N' cần được theo dõi.',
        DATEADD(hour, -ROW_NUMBER() OVER (ORDER BY lt.maLichTiem) * 6, GETDATE()),
        CASE WHEN ROW_NUMBER() OVER (ORDER BY lt.maLichTiem) % 3 = 0 THEN 1 ELSE 0 END
    FROM dbo.LichTiem lt
    JOIN dbo.HoSoSucKhoe hs ON hs.maHoSo = lt.maHoSo
    JOIN dbo.MuiTiemVaccine mt ON mt.maMuiTiem = lt.maMuiTiem
    JOIN dbo.Vaccine vc ON vc.maVaccine = mt.maVaccine
    ORDER BY lt.maLichTiem;

    INSERT INTO dbo.BaiVietCamNang(maTaiKhoan, tieuDe, slug, moTaNgan, noiDung, loaiBaiViet, anhDaiDien, ngayTao, trangThai, noiBat, luotXem)
    VALUES
    (1, N'Những điều cần biết trước khi tiêm vaccine', N'nhung-dieu-can-biet-truoc-khi-tiem-vaccine', N'Các bước chuẩn bị giúp buổi tiêm diễn ra an toàn.', N'Người đi tiêm nên mang theo sổ tiêm, thông tin bệnh nền, thuốc đang sử dụng và báo ngay cho nhân viên y tế nếu từng có phản ứng dị ứng. Trước ngày tiêm nên ngủ đủ, ăn nhẹ và theo dõi tình trạng sốt hoặc bệnh cấp tính.', N'Cẩm nang sức khỏe', N'/images/cam-nang/vaccine-checklist.jpg', DATEADD(day, -20, GETDATE()), 1, 1, 156),
    (1, N'Cách theo dõi phản ứng sau tiêm', N'cach-theo-doi-phan-ung-sau-tiem', N'Dấu hiệu thường gặp và khi nào cần liên hệ bác sĩ.', N'Sau tiêm cần ở lại điểm tiêm theo thời gian được hướng dẫn. Tại nhà, tiếp tục theo dõi sốt, sưng đau chỗ tiêm, phát ban, khó thở hoặc mệt bất thường. Nếu có biểu hiện nặng cần đưa người tiêm đến cơ sở y tế.', N'Cẩm nang sức khỏe', N'/images/cam-nang/theo-doi-sau-tiem.jpg', DATEADD(day, -19, GETDATE()), 1, 1, 132),
    (1, N'Lịch tiêm chủng cơ bản cho trẻ nhỏ', N'lich-tiem-chung-co-ban-cho-tre-nho', N'Gợi ý các mốc tiêm quan trọng trong những năm đầu đời.', N'Trẻ nhỏ cần được tiêm theo mốc tuổi để tạo miễn dịch đúng thời điểm. Cha mẹ nên lưu lịch tiêm, đặt nhắc hẹn và hỏi nhân viên y tế khi trẻ bị sốt hoặc trễ lịch.', N'Cẩm nang sức khỏe', N'/images/cam-nang/lich-tiem-tre-nho.jpg', DATEADD(day, -18, GETDATE()), 1, 1, 188),
    (1, N'Vì sao cần tiêm nhắc lại vaccine?', N'vi-sao-can-tiem-nhac-lai-vaccine', N'Tiêm nhắc giúp duy trì miễn dịch theo thời gian.', N'Một số vaccine cần mũi nhắc vì miễn dịch có thể giảm dần. Tiêm nhắc đúng lịch giúp cơ thể củng cố đáp ứng miễn dịch và giảm nguy cơ mắc bệnh nặng.', N'Cẩm nang sức khỏe', N'/images/cam-nang/tiem-nhac-lai.jpg', DATEADD(day, -17, GETDATE()), 1, 0, 97),
    (1, N'Vaccine cúm mùa và đối tượng nên tiêm', N'vaccine-cum-mua-va-doi-tuong-nen-tiem', N'Cúm mùa nên được phòng ngừa hằng năm.', N'Vaccine cúm mùa đặc biệt hữu ích cho trẻ nhỏ, người cao tuổi, phụ nữ mang thai, người có bệnh nền và nhân viên y tế. Nên tiêm trước mùa cúm để cơ thể có thời gian tạo kháng thể.', N'Cẩm nang sức khỏe', N'/images/cam-nang/cum-mua.jpg', DATEADD(day, -16, GETDATE()), 1, 0, 121),
    (1, N'Những lưu ý khi trẻ bị sốt sau tiêm', N'nhung-luu-y-khi-tre-bi-sot-sau-tiem', N'Sốt nhẹ thường gặp nhưng cần theo dõi đúng cách.', N'Cha mẹ nên đo nhiệt độ, cho trẻ uống đủ nước, mặc đồ thoáng và dùng thuốc hạ sốt theo hướng dẫn. Không tự ý đắp lá hoặc dùng thuốc không rõ nguồn gốc.', N'Cẩm nang sức khỏe', N'/images/cam-nang/tre-sot-sau-tiem.jpg', DATEADD(day, -15, GETDATE()), 1, 0, 110),
    (1, N'Vaccine HPV và độ tuổi khuyến nghị', N'vaccine-hpv-va-do-tuoi-khuyen-nghi', N'HPV nên được tiêm trong độ tuổi phù hợp để tối ưu hiệu quả.', N'Vaccine HPV giúp phòng các bệnh liên quan đến virus HPV. Nên trao đổi với bác sĩ để chọn phác đồ phù hợp theo tuổi, giới tính và tình trạng sức khỏe.', N'Cẩm nang sức khỏe', N'/images/cam-nang/hpv.jpg', DATEADD(day, -14, GETDATE()), 1, 0, 104),
    (1, N'Tiêm trễ lịch có cần tiêm lại từ đầu không?', N'tiem-tre-lich-co-can-tiem-lai-tu-dau-khong', N'Hầu hết trường hợp không cần bắt đầu lại toàn bộ phác đồ.', N'Khi bị trễ lịch, người dùng nên tiếp tục mũi còn thiếu sớm nhất có thể và hỏi nhân viên y tế về khoảng cách mũi tiếp theo. Không tự ý bỏ mũi hoặc tiêm dồn nhiều mũi.', N'Cẩm nang sức khỏe', N'/images/cam-nang/tre-lich.jpg', DATEADD(day, -13, GETDATE()), 1, 1, 174),
    (1, N'Cách quản lý lịch tiêm cho nhiều thành viên gia đình', N'cach-quan-ly-lich-tiem-cho-nhieu-thanh-vien-gia-dinh', N'Tổ chức hồ sơ giúp tránh bỏ sót mũi tiêm.', N'Mỗi thành viên nên có hồ sơ riêng với ngày sinh, tiền sử bệnh, dị ứng và lịch đã tiêm. Việc đặt nhắc hẹn giúp gia đình chủ động theo dõi lịch sắp đến và lịch quá hạn.', N'Cẩm nang sức khỏe', N'/images/cam-nang/quan-ly-gia-dinh.jpg', DATEADD(day, -12, GETDATE()), 1, 0, 89),
    (1, N'Khi nào cần hỏi ý kiến bác sĩ trước khi tiêm?', N'khi-nao-can-hoi-y-kien-bac-si-truoc-khi-tiem', N'Một số tình trạng sức khỏe cần được tư vấn trước tiêm.', N'Người có tiền sử phản vệ, bệnh cấp tính nặng, đang dùng thuốc ức chế miễn dịch, phụ nữ mang thai hoặc người có bệnh nền chưa ổn định nên hỏi ý kiến bác sĩ trước khi tiêm.', N'Cẩm nang sức khỏe', N'/images/cam-nang/hoi-y-kien-bac-si.jpg', DATEADD(day, -11, GETDATE()), 1, 1, 145);

    INSERT INTO dbo.CauHoiTuVan(maNguoiGui, maNguoiTraLoi, maVaccine, cauHoi, cauTraLoi, ngayGui, ngayTraLoi, trangThai)
    VALUES
    (2, 1, 7, N'Tôi bị viêm mũi dị ứng theo mùa thì có tiêm cúm được không?', N'Bạn vẫn có thể tiêm nếu không đang sốt hoặc có bệnh cấp tính. Khi đến tiêm hãy thông báo tiền sử dị ứng để được sàng lọc kỹ.', DATEADD(day, -12, GETDATE()), DATEADD(day, -11, GETDATE()), N'Đã trả lời'),
    (2, NULL, 10, N'Bé 3 tháng từng nổi mẩn sau khi ăn trứng có cần tư vấn trước khi tiêm phế cầu không?', NULL, DATEADD(day, -10, GETDATE()), NULL, N'Chưa trả lời'),
    (3, 1, 3, N'Trẻ bị trễ mũi 6 trong 1 hơn 2 tuần có cần tiêm lại từ đầu không?', N'Thông thường không cần tiêm lại từ đầu. Gia đình nên đưa trẻ đi tiêm mũi còn thiếu và cập nhật lại lịch mũi tiếp theo.', DATEADD(day, -9, GETDATE()), DATEADD(day, -8, GETDATE()), N'Đã trả lời'),
    (3, NULL, 1, N'Người lớn chưa rõ từng tiêm viêm gan B có nên xét nghiệm trước không?', NULL, DATEADD(day, -8, GETDATE()), NULL, N'Chưa trả lời'),
    (4, 1, 5, N'Con tôi 8 tuổi chưa tiêm MMR mũi 2 thì có tiêm bổ sung được không?', N'Có thể tiêm bổ sung nếu không có chống chỉ định. Bạn nên mang theo sổ tiêm để nhân viên y tế kiểm tra phác đồ.', DATEADD(day, -7, GETDATE()), DATEADD(day, -6, GETDATE()), N'Đã trả lời'),
    (4, NULL, 9, N'Trẻ bị viêm phế quản tái phát có cần hoãn vaccine thủy đậu không?', NULL, DATEADD(day, -6, GETDATE()), NULL, N'Chưa trả lời'),
    (5, 1, 8, N'Học sinh nam có nên tiêm HPV không?', N'Nam giới vẫn có thể được khuyến nghị tiêm HPV trong độ tuổi phù hợp. Bạn nên trao đổi thêm khi khám sàng lọc.', DATEADD(day, -5, GETDATE()), DATEADD(day, -4, GETDATE()), N'Đã trả lời'),
    (5, 1, 12, N'Người cao tuổi có bệnh nền nên tiêm Covid-19 nhắc lại khi nào?', N'Lịch nhắc phụ thuộc khuyến cáo hiện hành và tình trạng bệnh nền. Người cao tuổi nên được bác sĩ sàng lọc trước tiêm.', DATEADD(day, -5, GETDATE()), DATEADD(day, -3, GETDATE()), N'Đã đóng'),
    (6, NULL, 7, N'Tôi dị ứng hải sản nhẹ có cần ở lại theo dõi lâu hơn sau tiêm cúm không?', NULL, DATEADD(day, -4, GETDATE()), NULL, N'Chưa trả lời'),
    (6, 1, 11, N'Trẻ quá tuổi uống Rota thì có thể uống bù không?', N'Vaccine Rota có giới hạn tuổi khá chặt. Nếu trẻ đã quá tuổi, gia đình không nên tự ý uống bù và cần hỏi bác sĩ.', DATEADD(day, -3, GETDATE()), DATEADD(day, -2, GETDATE()), N'Đã trả lời'),
    (7, NULL, 8, N'Nữ 27 tuổi chưa từng tiêm HPV thì bắt đầu phác đồ mấy mũi?', NULL, DATEADD(day, -2, GETDATE()), NULL, N'Chưa trả lời'),
    (8, 1, 10, N'Người bệnh phổi mạn tính có nên ưu tiên vaccine phế cầu không?', N'Người có bệnh phổi mạn tính thường thuộc nhóm nên được tư vấn tiêm phế cầu. Bác sĩ sẽ quyết định phác đồ phù hợp sau sàng lọc.', DATEADD(day, -1, GETDATE()), GETDATE(), N'Đã trả lời');

    IF (SELECT COUNT(*) FROM dbo.LichTiem) <> 75
        THROW 51001, N'Seed LichTiem failed: expected 75 rows.', 1;
    IF (SELECT COUNT(*) FROM dbo.LichSuTiem) <> 25
        THROW 51002, N'Seed LichSuTiem failed: expected 25 rows.', 1;

    COMMIT TRANSACTION;

    SELECT N'Database thao tác' AS NoiDung, DB_NAME() AS GiaTri;
    SELECT TenBang, SoLuong AS SoLuongDaXoa FROM @BeforeCounts ORDER BY TenBang;
    SELECT N'TaiKhoan' AS TenBang, COUNT(*) AS SoLuongSauSeed FROM dbo.TaiKhoan UNION ALL
    SELECT N'HoSoSucKhoe', COUNT(*) FROM dbo.HoSoSucKhoe UNION ALL
    SELECT N'Vaccine', COUNT(*) FROM dbo.Vaccine UNION ALL
    SELECT N'MuiTiemVaccine', COUNT(*) FROM dbo.MuiTiemVaccine UNION ALL
    SELECT N'LichTiem', COUNT(*) FROM dbo.LichTiem UNION ALL
    SELECT N'LichSuTiem', COUNT(*) FROM dbo.LichSuTiem UNION ALL
    SELECT N'ThongBao', COUNT(*) FROM dbo.ThongBao UNION ALL
    SELECT N'BaiVietCamNang', COUNT(*) FROM dbo.BaiVietCamNang UNION ALL
    SELECT N'CauHoiTuVan', COUNT(*) FROM dbo.CauHoiTuVan;

    SELECT N'Admin login' AS Loai, soDienThoai, email, N'Password123' AS MatKhauMau
    FROM dbo.TaiKhoan
    WHERE vaiTro = N'Admin';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage nvarchar(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity int = ERROR_SEVERITY();
    DECLARE @ErrorState int = ERROR_STATE();
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;
GO
