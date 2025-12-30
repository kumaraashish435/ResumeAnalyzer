-- SQL Database Schema for AI Resume Analyzer
-- This schema is for reference. EF Core creates the schema automatically.
-- For SQL Server, you can use this as a reference.

-- Users Table
CREATE TABLE [Users] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL UNIQUE,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Skills Master Table
CREATE TABLE [Skills] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [Category] NVARCHAR(100),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Resumes Table
CREATE TABLE [Resumes] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [FileName] NVARCHAR(500) NOT NULL,
    [FilePath] NVARCHAR(1000) NOT NULL,
    [ExtractedText] NTEXT,
    [FileSize] BIGINT NOT NULL,
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ProcessedAt] DATETIME2,
    CONSTRAINT [FK_Resumes_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

-- Job Descriptions Table
CREATE TABLE [JobDescriptions] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [JobTitle] NVARCHAR(200) NOT NULL,
    [Description] NTEXT NOT NULL,
    [Company] NVARCHAR(200),
    [Location] NVARCHAR(200),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Resume Skills Junction Table (Many-to-Many)
CREATE TABLE [ResumeSkills] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [ResumeId] INT NOT NULL,
    [SkillId] INT NOT NULL,
    [ConfidenceScore] FLOAT NOT NULL,
    [ExtractedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_ResumeSkills_Resumes] FOREIGN KEY ([ResumeId]) REFERENCES [Resumes]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ResumeSkills_Skills] FOREIGN KEY ([SkillId]) REFERENCES [Skills]([Id]) ON DELETE RESTRICT,
    CONSTRAINT [UQ_ResumeSkills_Resume_Skill] UNIQUE ([ResumeId], [SkillId])
);

-- Job Skills Junction Table (Many-to-Many)
CREATE TABLE [JobSkills] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [JobDescriptionId] INT NOT NULL,
    [SkillId] INT NOT NULL,
    [IsRequired] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_JobSkills_JobDescriptions] FOREIGN KEY ([JobDescriptionId]) REFERENCES [JobDescriptions]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_JobSkills_Skills] FOREIGN KEY ([SkillId]) REFERENCES [Skills]([Id]) ON DELETE RESTRICT,
    CONSTRAINT [UQ_JobSkills_Job_Skill] UNIQUE ([JobDescriptionId], [SkillId])
);

-- Matching Results Table
CREATE TABLE [MatchingResults] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [ResumeId] INT NOT NULL,
    [JobDescriptionId] INT NOT NULL,
    [MatchPercentage] FLOAT NOT NULL,
    [CosineSimilarityScore] FLOAT NOT NULL,
    [MatchingSkillsCount] INT NOT NULL,
    [TotalJobSkillsCount] INT NOT NULL,
    [SkillMatchScore] FLOAT NOT NULL,
    [MatchedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Metadata] NVARCHAR(MAX),
    CONSTRAINT [FK_MatchingResults_Resumes] FOREIGN KEY ([ResumeId]) REFERENCES [Resumes]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MatchingResults_JobDescriptions] FOREIGN KEY ([JobDescriptionId]) REFERENCES [JobDescriptions]([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_MatchingResults_Resume_Job] UNIQUE ([ResumeId], [JobDescriptionId])
);

-- Indexes for Performance
CREATE INDEX [IX_Resumes_UserId] ON [Resumes]([UserId]);
CREATE INDEX [IX_ResumeSkills_ResumeId] ON [ResumeSkills]([ResumeId]);
CREATE INDEX [IX_ResumeSkills_SkillId] ON [ResumeSkills]([SkillId]);
CREATE INDEX [IX_JobSkills_JobDescriptionId] ON [JobSkills]([JobDescriptionId]);
CREATE INDEX [IX_JobSkills_SkillId] ON [JobSkills]([SkillId]);
CREATE INDEX [IX_MatchingResults_ResumeId] ON [MatchingResults]([ResumeId]);
CREATE INDEX [IX_MatchingResults_JobDescriptionId] ON [MatchingResults]([JobDescriptionId]);

