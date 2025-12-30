using Microsoft.EntityFrameworkCore.Storage;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Data.Repositories;


/// Unit of Work Implementation
/// Coordinates multiple repositories and manages database transactions
/// Ensures atomicity - either all changes succeed or none do
/// Creates repository instances and wraps SaveChanges in transaction management

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    // Lazy initialization of repositories (created only when accessed)
    private IRepository<User>? _users;
    private IResumeRepository? _resumes;
    private IJobDescriptionRepository? _jobDescriptions;
    private IRepository<Skill>? _skills;
    private IRepository<ResumeSkill>? _resumeSkills;
    private IRepository<JobSkill>? _jobSkills;
    private IRepository<MatchingResult>? _matchingResults;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users
    {
        get
        {
            _users ??= new Repository<User>(_context);
            return _users;
        }
    }

    public IResumeRepository Resumes
    {
        get
        {
            _resumes ??= new ResumeRepository(_context);
            return _resumes;
        }
    }

    public IJobDescriptionRepository JobDescriptions
    {
        get
        {
            _jobDescriptions ??= new JobDescriptionRepository(_context);
            return _jobDescriptions;
        }
    }

    public IRepository<Skill> Skills
    {
        get
        {
            _skills ??= new Repository<Skill>(_context);
            return _skills;
        }
    }

    public IRepository<ResumeSkill> ResumeSkills
    {
        get
        {
            _resumeSkills ??= new Repository<ResumeSkill>(_context);
            return _resumeSkills;
        }
    }

    public IRepository<JobSkill> JobSkills
    {
        get
        {
            _jobSkills ??= new Repository<JobSkill>(_context);
            return _jobSkills;
        }
    }

    public IRepository<MatchingResult> MatchingResults
    {
        get
        {
            _matchingResults ??= new Repository<MatchingResult>(_context);
            return _matchingResults;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

