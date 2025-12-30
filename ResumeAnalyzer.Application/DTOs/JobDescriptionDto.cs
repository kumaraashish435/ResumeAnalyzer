namespace ResumeAnalyzer.Application.DTOs;


/// Data Transfer Object for Job Description
/// Represents job description data for display and input
/// Separates presentation concerns from domain model
/// Contains job information with list of required skills

public class JobDescriptionDto
{
    public int Id { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

