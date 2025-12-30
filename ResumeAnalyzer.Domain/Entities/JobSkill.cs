namespace ResumeAnalyzer.Domain.Entities;


/// Domain Entity: JobSkill (Mapping Table)
/// Many-to-many relationship mapping between JobDescriptions and Skills
/// A job can require multiple skills, and a skill can be required by multiple jobs
/// Junction table connecting JobDescription and Skill entities

public class JobSkill
{
    public int Id { get; set; }
    
    
    /// Foreign key to the JobDescription
    
    public int JobDescriptionId { get; set; }
    
    
    /// Foreign key to the Skill
    
    public int SkillId { get; set; }
    
    
    /// Whether this skill is required (true) or preferred (false) for the job
    
    public bool IsRequired { get; set; }
    
    
    /// Navigation property: The job description this skill belongs to
    
    public JobDescription JobDescription { get; set; } = null!;
    
    
    /// Navigation property: The skill entity
    
    public Skill Skill { get; set; } = null!;
}

