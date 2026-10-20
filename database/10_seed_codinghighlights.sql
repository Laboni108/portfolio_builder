USE PortfolioDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CodingHighlights)
BEGIN
    INSERT INTO dbo.CodingHighlights (Icon, Title, DisplayOrder)
    VALUES
    (NCHAR(0x26A1),
     N'100+ Problems Solved on Codeforces', 1),

    (NCHAR(0xD83D) + NCHAR(0xDCBB),
     N'LeetCode (Easy & Medium Problems)', 2),

    (NCHAR(0xD83C) + NCHAR(0xDFC6),
     N'23rd Place ' + NCHAR(0x2014) + N' Inter KUET Programming Contest (IKPC) 2022', 3),

    (NCHAR(0xD83E) + NCHAR(0xDDE0),
     N'Strong Foundation in Data Structures & Algorithms', 4);
END
GO