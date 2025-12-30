using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Data.Repositories;


/// Resume Repository Implementation
/// Implements specialized resume operations
/// Provides efficient queries with related data loaded (eager loading)
/// Uses Include() to load related entities in a single database query

public class ResumeRepository : Repository<Resume>, IResumeRepository
{
    public ResumeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Resume?> GetResumeWithDetailsAsync(int resumeId)
    {
        return await _dbSet
            .Include(r => r.User)
            .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill)
            .Include(r => r.MatchingResults)
                .ThenInclude(mr => mr.JobDescription)
            .FirstOrDefaultAsync(r => r.Id == resumeId);
    }

    public async Task<IEnumerable<Resume>> GetResumesByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill)
            .OrderByDescending(r => r.UploadedAt)
            .ToListAsync();
    }

    public async Task<Resume?> GetResumeWithSkillsAsync(int resumeId)
    {
        return await _dbSet
            .Include(r => r.ResumeSkills)
                .ThenInclude(rs => rs.Skill)
            .FirstOrDefaultAsync(r => r.Id == resumeId);
    }
}

