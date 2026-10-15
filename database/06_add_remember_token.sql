USE PortfolioDb;
GO

IF COL_LENGTH('dbo.AdminUsers', 'RememberTokenHash') IS NULL
BEGIN
    ALTER TABLE dbo.AdminUsers
    ADD RememberTokenHash      NVARCHAR(100) NULL,
        RememberTokenExpiresAt DATETIME2     NULL;
END
GO