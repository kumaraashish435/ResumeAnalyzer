namespace ResumeAnalyzer.Domain.Entities;


/// Domain Entity: MatchingResult
/// Stores the result of matching a resume against a job description
/// Persists match scores and metadata for historical tracking and analysis
/// Contains match percentage, TF-IDF vectors, cosine similarity score, and timestamps

public class MatchingResult
{
    public int Id { get; set; }
    
    
    /// Foreign key to the Resume being matched
    
    public int ResumeId { get; set; }
    
    
    /// Foreign key to the JobDescription being matched against
    
    public int JobDescriptionId { get; set; }
    
    
    /// Match percentage score (0-100) calculated using cosine similarity and other factors
    /// Higher score means better match
    
    public double MatchPercentage { get; set; }
    
    
    /// Raw cosine similarity score (0-1) from TF-IDF vector comparison
    /// This is the mathematical similarity score before conversion to percentage
    
    public double CosineSimilarityScore { get; set; }
    
    
    /// Number of matching skills found between resume and job
    
    public int MatchingSkillsCount { get; set; }
    
    
    /// Total number of skills required/preferred by the job
    
    public int TotalJobSkillsCount { get; set; }
    
    
    /// Skill-based match score (0-100) based on skill overlap
    
    public double SkillMatchScore { get; set; }
    
    
    /// Timestamp when the matching was performed
    
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
    
    
    /// Additional metadata or notes about the match (JSON format for extensibility)
    
    public string? Metadata { get; set; }
    
    
    /// Navigation property: The resume that was matched
    
    public Resume Resume { get; set; } = null!;
    
    
    /// Navigation property: The job description that was matched against
    
    public JobDescription JobDescription { get; set; } = null!;
}

