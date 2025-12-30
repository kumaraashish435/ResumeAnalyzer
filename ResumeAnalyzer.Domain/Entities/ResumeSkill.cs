namespace ResumeAnalyzer.Domain.Entities;


/// Domain Entity: ResumeSkill (Mapping Table)
/// Many-to-many relationship mapping between Resumes and Skills
/// A resume can have multiple skills, and a skill can appear in multiple resumes
/// Junction table connecting Resume and Skill entities with optional confidence score

public class ResumeSkill
{
    public int Id { get; set; }
    
    
    /// Foreign key to the Resume
    
    public int ResumeId { get; set; }
    
    
    /// Foreign key to the Skill
    
    public int SkillId { get; set; }
    
    
    /// Confidence score (0-1) indicating how certain the AI is that this skill exists in the resume
    /// Higher score means higher confidence
    
    public double ConfidenceScore { get; set; }
    
    
    /// Timestamp when this skill was extracted and associated with the resume
    
    public DateTime ExtractedAt { get; set; } = DateTime.UtcNow;
    
    
    /// Navigation property: The resume this skill belongs to
    
    public Resume Resume { get; set; } = null!;
    
    
    /// Navigation property: The skill entity
    
    public Skill Skill { get; set; } = null!;
}

