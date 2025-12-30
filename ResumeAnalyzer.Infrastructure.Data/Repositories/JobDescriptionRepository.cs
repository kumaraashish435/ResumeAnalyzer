using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Data.Repositories;


/// Job Description Repository Implementation
/// Implements specialized job description operations
/// Efficiently loads job descriptions with their required skills
/// Uses Include() for eager loading of JobSkills and Skills

public class JobDescriptionRepository : Repository<JobDescription>, IJobDescriptionRepository
{
    public JobDescriptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<JobDescription?> GetJobWithSkillsAsync(int jobId)
    {
        return await _dbSet
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync(j => j.Id == jobId);
    }

    public async Task<IEnumerable<JobDescription>> GetAllJobsWithSkillsAsync()
    {
        return await _dbSet
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }
}

