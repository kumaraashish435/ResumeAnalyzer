using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Data.Repositories;


/// Unit of Work Pattern Interface
/// Manages transactions and coordinates multiple repositories
/// Ensures data consistency by saving all changes together in a single transaction
/// Groups repository operations and provides a single SaveChanges() method

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IResumeRepository Resumes { get; }
    IJobDescriptionRepository JobDescriptions { get; }
    IRepository<Skill> Skills { get; }
    IRepository<ResumeSkill> ResumeSkills { get; }
    IRepository<JobSkill> JobSkills { get; }
    IRepository<MatchingResult> MatchingResults { get; }
    
    
    /// Save all changes to the database in a single transaction
    
    Task<int> SaveChangesAsync();
    
    
    /// Begin a new database transaction
    
    Task BeginTransactionAsync();
    
    
    /// Commit the current transaction
    
    Task CommitTransactionAsync();
    
    
    /// Rollback the current transaction
    
    Task RollbackTransactionAsync();
}

