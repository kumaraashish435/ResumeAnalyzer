namespace ResumeAnalyzer.Domain.Entities;


/// Domain Entity: User
/// Represents a user who can upload resumes and match them with job descriptions
/// Needed to track who is using the system and associate resumes with users
/// Stores user identification information

public class User
{
    public int Id { get; set; }
    
    
    /// User's full name for identification
    
    public string Name { get; set; } = string.Empty;
    
    
    /// Email address for user identification and login
    
    public string Email { get; set; } = string.Empty;
    
    
    /// Timestamp when the user was created
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    
    /// Navigation property: Collection of resumes uploaded by this user
    
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}

