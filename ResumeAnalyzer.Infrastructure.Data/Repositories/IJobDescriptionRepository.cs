using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Data.Repositories;


/// Job Description Repository Interface
/// Defines specialized operations for JobDescription entity
/// Provides job-specific queries with related skills
/// Extends IRepository with job-specific methods

public interface IJobDescriptionRepository : IRepository<JobDescription>
{
    
    /// Get job description with all required skills
    
    Task<JobDescription?> GetJobWithSkillsAsync(int jobId);
    
    
    /// Get all job descriptions with their skills
    
    Task<IEnumerable<JobDescription>> GetAllJobsWithSkillsAsync();
}

