namespace ResumeAnalyzer.Domain.Entities;


/// Domain Entity: JobDescription
/// Represents a job description/requirement against which resumes are matched
/// Needed to store job requirements and descriptions for resume matching
/// Contains job title, description text, and relationships to skills and matching results

public class JobDescription
{
    public int Id { get; set; }
    
    
    /// Job title (e.g., "Senior Software Engineer", "Data Analyst")
    
    public string JobTitle { get; set; } = string.Empty;
    
    
    /// Full job description text containing requirements, responsibilities, etc.
    
    public string Description { get; set; } = string.Empty;
    
    
    /// Company name offering this job
    
    public string Company { get; set; } = string.Empty;
    
    
    /// Location of the job
    
    public string Location { get; set; } = string.Empty;
    
    
    /// Timestamp when the job description was created
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    
    /// Navigation property: Skills required for this job
    
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    
    
    /// Navigation property: Matching results for this job
    
    public ICollection<MatchingResult> MatchingResults { get; set; } = new List<MatchingResult>();
}

