USE PortfolioDb;
GO

IF OBJECT_ID('dbo.AdminUsers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AdminUsers (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(50)  NOT NULL UNIQUE,
        PasswordHash NVARCHAR(200) NOT NULL,
        CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO