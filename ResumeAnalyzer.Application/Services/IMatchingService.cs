using ResumeAnalyzer.Application.DTOs;

namespace ResumeAnalyzer.Application.Services;


/// Matching Service Interface
/// Defines operations for matching resumes against job descriptions
/// Encapsulates the core matching algorithm and result generation
/// Coordinates AI services to calculate similarity and generate match results

public interface IMatchingService
{
    
    /// Match a resume against a job description
    
    Task<MatchingResultDto> MatchResumeToJobAsync(int resumeId, int jobDescriptionId);
    
    
    /// Match all resumes against a specific job description
    
    Task<IEnumerable<MatchingResultDto>> MatchAllResumesToJobAsync(int jobDescriptionId);
    
    
    /// Match a specific resume against all job descriptions
    
    Task<IEnumerable<MatchingResultDto>> MatchResumeToAllJobsAsync(int resumeId);
    
    
    /// Get existing matching result if available
    
    Task<MatchingResultDto?> GetMatchingResultAsync(int resumeId, int jobDescriptionId);
}

