using ResumeAnalyzer.Application.DTOs;

namespace ResumeAnalyzer.Application.Services;


/// Job Description Service Interface
/// Defines business logic operations for job description management
/// Encapsulates job description creation and skill association logic
/// Coordinates between data access and business rules

public interface IJobDescriptionService
{
    
    /// Create a new job description with skills
    
    Task<JobDescriptionDto> CreateJobDescriptionAsync(CreateJobDescriptionDto createDto);
    
    
    /// Get all job descriptions
    
    Task<IEnumerable<JobDescriptionDto>> GetAllJobDescriptionsAsync();
    
    
    /// Get a specific job description by ID
    
    Task<JobDescriptionDto?> GetJobDescriptionByIdAsync(int jobId);
    
    
    /// Delete a job description
    
    Task<bool> DeleteJobDescriptionAsync(int jobId);
}

