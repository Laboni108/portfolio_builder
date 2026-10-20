USE PortfolioDb;
GO

IF OBJECT_ID('dbo.SkillCategories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SkillCategories (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Name         NVARCHAR(100) NOT NULL,
        IconClass    NVARCHAR(60)  NOT NULL,
        Items        NVARCHAR(500) NOT NULL,
        DisplayOrder INT           NOT NULL DEFAULT 0,
        CreatedAt    DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO