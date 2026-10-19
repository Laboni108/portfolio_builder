USE PortfolioDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SkillCategories)
BEGIN
    INSERT INTO dbo.SkillCategories (Name, IconClass, Items, DisplayOrder)
    VALUES
    (N'Languages',         N'fas fa-code',         N'C, C++, Java, Python, JavaScript, PHP, SQL', 1),
    (N'Frameworks',        N'fas fa-layer-group',  N'Laravel, ASP.NET, Flutter', 2),
    (N'Development Tools', N'fab fa-docker',       N'Linux/Ubuntu, Git, GitHub, REST APIs, OMNeT++, Android Studio', 3);
END
GO