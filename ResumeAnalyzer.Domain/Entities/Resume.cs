namespace ResumeAnalyzer.Domain.Entities;


/// Domain Entity: Resume
/// Represents a PDF resume document uploaded by a user
/// Core entity that stores resume information and extracted text for matching
/// Contains file metadata, extracted text, and relationships to skills and matching results

public class Resume
{
    public int Id { get; set; }
    
    
    /// Foreign key to the User who uploaded this resume
    
    public int UserId { get; set; }
    
    
    /// Original filename of the uploaded PDF
    
    public string FileName { get; set; } = string.Empty;
    
    
    /// Full path where the PDF file is stored on the server
    
    public string FilePath { get; set; } = string.Empty;
    
    
    /// Extracted and cleaned text content from the PDF resume
    
    public string ExtractedText { get; set; } = string.Empty;
    
    
    /// File size in bytes
    
    public long FileSize { get; set; }
    
    
    /// Timestamp when the resume was uploaded
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    
    /// Timestamp when the resume text was last processed
    
    public DateTime? ProcessedAt { get; set; }
    
    
    /// Navigation property: User who uploaded this resume
    
    public User User { get; set; } = null!;
    
    
    /// Navigation property: Skills extracted from this resume
    
    public ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
    
    
    /// Navigation property: Matching results for this resume
    
    public ICollection<MatchingResult> MatchingResults { get; set; } = new List<MatchingResult>();
}

