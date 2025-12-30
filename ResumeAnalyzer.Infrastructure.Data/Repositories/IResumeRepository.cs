using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Data.Repositories;


/// Resume Repository Interface
/// Defines specialized operations for Resume entity beyond basic CRUD
/// Provides domain-specific queries and operations that are commonly needed
/// Extends IRepository with resume-specific methods

public interface IResumeRepository : IRepository<Resume>
{
    
    /// Get resume with all related data (User, Skills, MatchingResults)
    
    Task<Resume?> GetResumeWithDetailsAsync(int resumeId);
    
    
    /// Get all resumes for a specific user
    
    Task<IEnumerable<Resume>> GetResumesByUserIdAsync(int userId);
    
    
    /// Get resume with extracted skills
    
    Task<Resume?> GetResumeWithSkillsAsync(int resumeId);
}

