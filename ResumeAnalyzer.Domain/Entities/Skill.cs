namespace ResumeAnalyzer.Domain.Entities;


/// Domain Entity: Skill
/// Master table storing all available skills (e.g., "C#", "Python", "SQL", "Azure")
/// Centralized skill dictionary for consistent skill identification and matching
/// Stores skill name and category, referenced by ResumeSkill and JobSkill mapping tables

public class Skill
{
    public int Id { get; set; }
    
    
    /// Name of the skill (e.g., "C#", "Machine Learning", "Project Management")
    
    public string Name { get; set; } = string.Empty;
    
    
    /// Category of the skill (e.g., "Programming", "Framework", "Soft Skill")
    /// Helps in grouping and organizing skills
    
    public string Category { get; set; } = string.Empty;
    
    
    /// Timestamp when the skill was added to the master list
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    
    /// Navigation property: Resumes that have this skill
    
    public ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
    
    
    /// Navigation property: Jobs that require this skill
    
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
}

