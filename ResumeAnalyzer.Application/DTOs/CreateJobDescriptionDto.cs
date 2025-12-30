using System.ComponentModel.DataAnnotations;

namespace ResumeAnalyzer.Application.DTOs;


/// Data Transfer Object for creating a new job description
/// Carries job description data from form submission
/// Validates input and separates concerns
/// Contains validation attributes and skill list

public class CreateJobDescriptionDto
{
    [Required(ErrorMessage = "Job title is required")]
    [StringLength(200, ErrorMessage = "Job title cannot exceed 200 characters")]
    public string JobTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job description is required")]
    public string Description { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
    public string Company { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
    public string Location { get; set; } = string.Empty;

    
    /// Comma-separated list of required skills
    
    public string RequiredSkills { get; set; } = string.Empty;
}

