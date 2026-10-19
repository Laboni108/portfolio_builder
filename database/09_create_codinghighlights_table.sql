USE PortfolioDb;
GO

IF OBJECT_ID('dbo.CodingHighlights', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CodingHighlights (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Icon         NVARCHAR(20)  NOT NULL,
        Title        NVARCHAR(200) NOT NULL,
        DisplayOrder INT           NOT NULL DEFAULT 0,
        CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO