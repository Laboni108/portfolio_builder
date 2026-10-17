USE PortfolioDb;
GO

IF OBJECT_ID('dbo.ContactMessages', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContactMessages (
        Id        INT IDENTITY(1,1) PRIMARY KEY,
        Name      NVARCHAR(100)  NOT NULL,
        Email     NVARCHAR(150)  NOT NULL,
        Subject   NVARCHAR(200)  NULL,
        Message   NVARCHAR(2000) NOT NULL,
        IsRead    BIT            NOT NULL DEFAULT 0,
        CreatedAt DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO