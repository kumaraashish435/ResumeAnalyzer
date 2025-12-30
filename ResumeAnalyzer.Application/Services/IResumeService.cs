using ResumeAnalyzer.Application.DTOs;

namespace ResumeAnalyzer.Application.Services;


/// Resume Service Interface
/// Defines business logic operations for resume management
/// Encapsulates complex resume processing workflows
/// Coordinates between data access, AI services, and business rules

public interface IResumeService
{
    
    /// Upload and process a resume PDF file
    
    Task<ResumeDto> UploadResumeAsync(ResumeUploadDto uploadDto, string uploadsFolderPath);
    
    
    /// Get all resumes for a user
    
    Task<IEnumerable<ResumeDto>> GetResumesByUserIdAsync(int userId);
    
    
    /// Get a specific resume by ID
    
    Task<ResumeDto?> GetResumeByIdAsync(int resumeId);
    
    
    /// Delete a resume
    
    Task<bool> DeleteResumeAsync(int resumeId);
}

