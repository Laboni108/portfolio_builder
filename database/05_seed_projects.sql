USE PortfolioDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Projects)
BEGIN
    INSERT INTO dbo.Projects (Title, Description, TechStack, GithubUrl)
    VALUES
    (N'HemoCam',
     N'Research-based mobile application for non-invasive hemoglobin estimation using skin and nail image analysis with machine learning and computer vision.',
     N'Java, Python, Scikit-learn, ElasticNet',
     N'https://github.com/Laboni108/Hemocam_web_APP'),

    (N'Voice Navigator',
     N'Accessibility-focused web navigation system that enables hands-free browser control using multilingual voice commands and real-time speech recognition.',
     N'JavaScript, Python, Web Speech API',
     N'https://github.com/Laboni108/Digital_Access_Bangla_Web_Brrowser'),

    (N'Human & Car Detection',
     N'Computer vision system using YOLOv8 to detect and count humans and vehicles from drone imagery with real-time visualization and OpenCV.',
     N'Python, YOLOv8, OpenCV, Kaggle',
     NULL),

    (N'Portfolio Website',
     N'Responsive full-stack portfolio website with a secure admin dashboard, SQL Server integration, CRUD functionality, and authentication.',
     N'C#, SQL Server, JavaScript, CSS',
     NULL);
END
GO