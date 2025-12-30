namespace ResumeAnalyzer.Application.DTOs;


/// Data Transfer Object for Resume information
/// Represents resume data for display in views
/// Provides a clean view model without exposing database internals
/// Contains only necessary fields for display

public class ResumeDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
}

