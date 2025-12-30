using System.Linq;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Infrastructure.Data.Repositories;

namespace ResumeAnalyzer.Application.Services;


/// Job Description Service Implementation
/// Implements business logic for job description operations
/// Handles job creation, skill association, and data validation
/// Creates job entities, processes skill lists, and manages relationships

public class JobDescriptionService : IJobDescriptionService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobDescriptionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    
    /// Create a new job description with skills
    
    public async Task<JobDescriptionDto> CreateJobDescriptionAsync(CreateJobDescriptionDto createDto)
    {
        // Create job description entity
        var jobDescription = new JobDescription
        {
            JobTitle = createDto.JobTitle,
            Description = createDto.Description,
            Company = createDto.Company,
            Location = createDto.Location,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.JobDescriptions.AddAsync(jobDescription);
        await _unitOfWork.SaveChangesAsync();

        // Process and add skills
        if (!string.IsNullOrWhiteSpace(createDto.RequiredSkills))
        {
            var skillNames = createDto.RequiredSkills
                .Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var allSkills = await _unitOfWork.Skills.GetAllAsync();
            var skillDict = allSkills.ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);

            foreach (var skillName in skillNames)
            {
                // Get or create skill
                Skill skill;
                if (skillDict.ContainsKey(skillName))
                {
                    skill = skillDict[skillName];
                }
                else
                {
                    // Create new skill if it doesn't exist
                    skill = new Skill
                    {
                        Name = skillName,
                        Category = "Technical", // Default category
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Skills.AddAsync(skill);
                    await _unitOfWork.SaveChangesAsync();
                    skillDict[skillName] = skill;
                }

                // Associate skill with job
                var jobSkill = new JobSkill
                {
                    JobDescriptionId = jobDescription.Id,
                    SkillId = skill.Id,
                    IsRequired = true
                };
                await _unitOfWork.JobSkills.AddAsync(jobSkill);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        // Fetch job with skills to return complete data
        var jobWithSkills = await _unitOfWork.JobDescriptions.GetJobWithSkillsAsync(jobDescription.Id);
        
        return MapToDto(jobWithSkills!);
    }

    
    /// Get all job descriptions
    
    public async Task<IEnumerable<JobDescriptionDto>> GetAllJobDescriptionsAsync()
    {
        var jobs = await _unitOfWork.JobDescriptions.GetAllJobsWithSkillsAsync();
        return jobs.Select(MapToDto);
    }

    
    /// Get a specific job description by ID
    
    public async Task<JobDescriptionDto?> GetJobDescriptionByIdAsync(int jobId)
    {
        var job = await _unitOfWork.JobDescriptions.GetJobWithSkillsAsync(jobId);
        if (job == null)
            return null;

        return MapToDto(job);
    }

    
    /// Delete a job description
    
    public async Task<bool> DeleteJobDescriptionAsync(int jobId)
    {
        var job = await _unitOfWork.JobDescriptions.GetByIdAsync(jobId);
        if (job == null)
            return false;

        await _unitOfWork.JobDescriptions.DeleteAsync(job);
        return true;
    }

    
    /// Map JobDescription entity to JobDescriptionDto
    
    private JobDescriptionDto MapToDto(JobDescription job)
    {
        return new JobDescriptionDto
        {
            Id = job.Id,
            JobTitle = job.JobTitle,
            Description = job.Description,
            Company = job.Company,
            Location = job.Location,
            RequiredSkills = job.JobSkills.Select(js => js.Skill.Name).ToList(),
            CreatedAt = job.CreatedAt
        };
    }
}

