USE PortfolioDb;
GO

IF OBJECT_ID('dbo.Projects', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Projects (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Title       NVARCHAR(150)  NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        TechStack   NVARCHAR(300)  NOT NULL,
        GithubUrl   NVARCHAR(300)  NULL,
        CreatedAt   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO