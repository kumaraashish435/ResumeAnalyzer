namespace ResumeAnalyzer.Application.DTOs;


/// Data Transfer Object for Matching Result
/// Represents the result of matching a resume against a job description
/// Provides a clean view model for displaying match results
/// Contains match scores and related information

public class MatchingResultDto
{
    public int Id { get; set; }
    public int ResumeId { get; set; }
    public string ResumeFileName { get; set; } = string.Empty;
    public int JobDescriptionId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public double MatchPercentage { get; set; }
    public double CosineSimilarityScore { get; set; }
    public int MatchingSkillsCount { get; set; }
    public int TotalJobSkillsCount { get; set; }
    public double SkillMatchScore { get; set; }
    public DateTime MatchedAt { get; set; }
    public List<string> MatchingSkills { get; set; } = new();
    public List<string> MissingSkills { get; set; } = new();
}

