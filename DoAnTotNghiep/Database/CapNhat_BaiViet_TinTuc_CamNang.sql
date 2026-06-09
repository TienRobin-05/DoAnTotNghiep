USE doAnTotNghiep;
GO

IF COL_LENGTH('dbo.BaiVietCamNang', 'loaiBaiViet') IS NULL
BEGIN
    ALTER TABLE dbo.BaiVietCamNang
    ADD loaiBaiViet NVARCHAR(30) NOT NULL
        CONSTRAINT DF_BaiVietCamNang_LoaiBaiViet DEFAULT (NCHAR(67) + NCHAR(7849) + NCHAR(109) + NCHAR(32) + NCHAR(110) + NCHAR(97) + NCHAR(110) + NCHAR(103));
END
GO

DECLARE @TenDefault NVARCHAR(128);
SELECT @TenDefault = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c
    ON dc.parent_object_id = c.object_id
    AND dc.parent_column_id = c.column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.BaiVietCamNang')
AND c.name = N'loaiBaiViet';

IF @TenDefault IS NOT NULL
BEGIN
    EXEC(N'ALTER TABLE dbo.BaiVietCamNang DROP CONSTRAINT [' + @TenDefault + N']');
END

ALTER TABLE dbo.BaiVietCamNang
ADD CONSTRAINT DF_BaiVietCamNang_LoaiBaiViet
DEFAULT (NCHAR(67) + NCHAR(7849) + NCHAR(109) + NCHAR(32) + NCHAR(110) + NCHAR(97) + NCHAR(110) + NCHAR(103)) FOR loaiBaiViet;
GO

IF COL_LENGTH('dbo.BaiVietCamNang', 'anhDaiDien') IS NULL
BEGIN
    ALTER TABLE dbo.BaiVietCamNang
    ADD anhDaiDien NVARCHAR(500) NULL;
END
GO

UPDATE dbo.BaiVietCamNang
SET loaiBaiViet = NCHAR(67) + NCHAR(7849) + NCHAR(109) + NCHAR(32) + NCHAR(110) + NCHAR(97) + NCHAR(110) + NCHAR(103)
WHERE loaiBaiViet IS NULL OR LTRIM(RTRIM(loaiBaiViet)) = N'';
GO

UPDATE dbo.BaiVietCamNang
SET anhDaiDien = N'/images/articles/lich-tiem-tre-nho.svg',
    loaiBaiViet = NCHAR(67) + NCHAR(7849) + NCHAR(109) + NCHAR(32) + NCHAR(110) + NCHAR(97) + NCHAR(110) + NCHAR(103)
WHERE maBaiViet = 1;

UPDATE dbo.BaiVietCamNang
SET anhDaiDien = N'/images/articles/chuan-bi-truoc-tiem.svg',
    loaiBaiViet = NCHAR(67) + NCHAR(7849) + NCHAR(109) + NCHAR(32) + NCHAR(110) + NCHAR(97) + NCHAR(110) + NCHAR(103)
WHERE maBaiViet = 2;

UPDATE dbo.BaiVietCamNang
SET anhDaiDien = N'/images/articles/theo-doi-sau-tiem.svg',
    loaiBaiViet = NCHAR(84) + NCHAR(105) + NCHAR(110) + NCHAR(32) + NCHAR(116) + NCHAR(7913) + NCHAR(99)
WHERE maBaiViet = 3;

UPDATE dbo.BaiVietCamNang
SET anhDaiDien = N'/images/articles/lich-su-tiem-chung.svg',
    loaiBaiViet = NCHAR(67) + NCHAR(7849) + NCHAR(109) + NCHAR(32) + NCHAR(110) + NCHAR(97) + NCHAR(110) + NCHAR(103)
WHERE maBaiViet = 4;
GO
