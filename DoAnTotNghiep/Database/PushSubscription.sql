SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID(N'dbo.PushSubscription', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PushSubscription (
        maPushSubscription INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PushSubscription PRIMARY KEY,
        maTaiKhoan INT NOT NULL,
        endpoint NVARCHAR(MAX) NOT NULL,
        endpointHash NVARCHAR(64) NOT NULL,
        p256dh NVARCHAR(MAX) NOT NULL,
        auth NVARCHAR(MAX) NOT NULL,
        ngayTao DATETIME NOT NULL CONSTRAINT DF_PushSubscription_ngayTao DEFAULT GETDATE(),
        ngayCapNhat DATETIME NULL,
        isActive BIT NOT NULL CONSTRAINT DF_PushSubscription_isActive DEFAULT 1
    );
END;

IF COL_LENGTH(N'dbo.PushSubscription', N'endpointHash') IS NULL
BEGIN
    ALTER TABLE dbo.PushSubscription ADD endpointHash NVARCHAR(64) NULL;
END;

IF COL_LENGTH(N'dbo.PushSubscription', N'isActive') IS NULL
BEGIN
    ALTER TABLE dbo.PushSubscription
    ADD isActive BIT NOT NULL CONSTRAINT DF_PushSubscription_isActive DEFAULT 1;
END;

IF COL_LENGTH(N'dbo.PushSubscription', N'ngayCapNhat') IS NULL
BEGIN
    ALTER TABLE dbo.PushSubscription ADD ngayCapNhat DATETIME NULL;
END;

UPDATE dbo.PushSubscription
SET endpointHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CONVERT(VARBINARY(MAX), endpoint)), 2)
WHERE endpointHash IS NULL
AND endpoint IS NOT NULL;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_PushSubscription_endpointHash'
    AND object_id = OBJECT_ID(N'dbo.PushSubscription')
)
BEGIN
    CREATE UNIQUE INDEX UX_PushSubscription_endpointHash
    ON dbo.PushSubscription(endpointHash)
    WHERE endpointHash IS NOT NULL;
END;

IF OBJECT_ID(N'dbo.TaiKhoan', N'U') IS NOT NULL
AND COL_LENGTH(N'dbo.TaiKhoan', N'maTaiKhoan') IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_PushSubscription_TaiKhoan'
    AND parent_object_id = OBJECT_ID(N'dbo.PushSubscription')
)
BEGIN
    ALTER TABLE dbo.PushSubscription
    ADD CONSTRAINT FK_PushSubscription_TaiKhoan
    FOREIGN KEY (maTaiKhoan) REFERENCES dbo.TaiKhoan(maTaiKhoan);
END;
