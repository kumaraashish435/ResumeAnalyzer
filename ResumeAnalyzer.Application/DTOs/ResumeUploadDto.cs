using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ResumeAnalyzer.Application.DTOs;


/// Data Transfer Object for Resume Upload
/// Carries data from the presentation layer to the application layer during resume upload
/// Separates concerns by keeping domain entities isolated from presentation layer
/// Contains file information and validation attributes for model binding

public class ResumeUploadDto
{
    
    /// User ID who is uploading the resume
    
    [Required(ErrorMessage = "User ID is required")]
    public int UserId { get; set; }

    
    /// Uploaded PDF file
    
    [Required(ErrorMessage = "Please select a PDF file")]
    public IFormFile? File { get; set; }
}

