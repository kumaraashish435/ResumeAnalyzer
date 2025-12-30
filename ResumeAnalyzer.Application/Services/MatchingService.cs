using System.Linq;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Infrastructure.AI.Services;
using ResumeAnalyzer.Infrastructure.Data.Repositories;

namespace ResumeAnalyzer.Application.Services;


/// Matching Service Implementation
/// Implements the resume-job matching algorithm
/// Core business logic that determines how well a resume matches a job
/// Uses TF-IDF, cosine similarity, and skill matching to calculate match scores
/// 
/// Matching Algorithm:
/// 1. Extract and preprocess text from resume and job description
/// 2. Calculate cosine similarity using TF-IDF vectors
/// 3. Calculate skill overlap percentage
/// 4. Combine both scores to get final match percentage
/// 5. Store result in database for future reference

public class MatchingService : IMatchingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITextPreprocessingService _textPreprocessingService;
    private readonly ISimilarityMatchingService _similarityMatchingService;

    public MatchingService(
        IUnitOfWork unitOfWork,
        ITextPreprocessingService textPreprocessingService,
        ISimilarityMatchingService similarityMatchingService)
    {
        _unitOfWork = unitOfWork;
        _textPreprocessingService = textPreprocessingService;
        _similarityMatchingService = similarityMatchingService;
    }

    
    /// Match a resume against a job description
    
    public async Task<MatchingResultDto> MatchResumeToJobAsync(int resumeId, int jobDescriptionId)
    {
        // Check if result already exists
        var existingResult = await GetMatchingResultAsync(resumeId, jobDescriptionId);
        if (existingResult != null)
        {
            return existingResult;
        }

        // Load resume with skills
        var resume = await _unitOfWork.Resumes.GetResumeWithSkillsAsync(resumeId);
        if (resume == null)
            throw new ArgumentException($"Resume with ID {resumeId} not found", nameof(resumeId));

        // Load job with skills
        var job = await _unitOfWork.JobDescriptions.GetJobWithSkillsAsync(jobDescriptionId);
        if (job == null)
            throw new ArgumentException($"Job description with ID {jobDescriptionId} not found", nameof(jobDescriptionId));

        // Preprocess texts
        string preprocessedResumeText = _textPreprocessingService.PreprocessTextToString(resume.ExtractedText);
        string preprocessedJobText = _textPreprocessingService.PreprocessTextToString(job.Description);

        // Calculate cosine similarity
        double cosineSimilarity = _similarityMatchingService.CalculateCosineSimilarity(
            preprocessedResumeText, 
            preprocessedJobText);

        // Calculate skill matching
        var resumeSkillNames = resume.ResumeSkills.Select(rs => rs.Skill.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var jobSkillNames = job.JobSkills.Select(js => js.Skill.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        var matchingSkills = resumeSkillNames.Intersect(jobSkillNames, StringComparer.OrdinalIgnoreCase).ToList();
        var missingSkills = jobSkillNames.Except(resumeSkillNames, StringComparer.OrdinalIgnoreCase).ToList();
        
        int matchingSkillsCount = matchingSkills.Count;
        int totalJobSkillsCount = jobSkillNames.Count;

        // Calculate match percentage (combines cosine similarity and skill match)
        double matchPercentage = _similarityMatchingService.CalculateMatchPercentage(
            preprocessedResumeText,
            preprocessedJobText,
            matchingSkillsCount,
            totalJobSkillsCount);

        double skillMatchScore = totalJobSkillsCount > 0 
            ? (double)matchingSkillsCount / totalJobSkillsCount * 100 
            : 0;

        // Save matching result to database
        var matchingResult = new MatchingResult
        {
            ResumeId = resumeId,
            JobDescriptionId = jobDescriptionId,
            MatchPercentage = matchPercentage,
            CosineSimilarityScore = cosineSimilarity,
            MatchingSkillsCount = matchingSkillsCount,
            TotalJobSkillsCount = totalJobSkillsCount,
            SkillMatchScore = skillMatchScore,
            MatchedAt = DateTime.UtcNow
        };

        await _unitOfWork.MatchingResults.AddAsync(matchingResult);
        await _unitOfWork.SaveChangesAsync();

        // Map to DTO
        return new MatchingResultDto
        {
            Id = matchingResult.Id,
            ResumeId = resumeId,
            ResumeFileName = resume.FileName,
            JobDescriptionId = jobDescriptionId,
            JobTitle = job.JobTitle,
            Company = job.Company,
            MatchPercentage = matchPercentage,
            CosineSimilarityScore = cosineSimilarity,
            MatchingSkillsCount = matchingSkillsCount,
            TotalJobSkillsCount = totalJobSkillsCount,
            SkillMatchScore = skillMatchScore,
            MatchedAt = matchingResult.MatchedAt,
            MatchingSkills = matchingSkills,
            MissingSkills = missingSkills
        };
    }

    
    /// Match all resumes against a specific job description
    
    public async Task<IEnumerable<MatchingResultDto>> MatchAllResumesToJobAsync(int jobDescriptionId)
    {
        var resumes = await _unitOfWork.Resumes.GetAllAsync();
        var results = new List<MatchingResultDto>();

        foreach (var resume in resumes)
        {
            try
            {
                var result = await MatchResumeToJobAsync(resume.Id, jobDescriptionId);
                results.Add(result);
            }
            catch (Exception)
            {
                // Skip resumes that fail matching (e.g., missing extracted text)
                continue;
            }
        }

        // Sort by match percentage descending
        return results.OrderByDescending(r => r.MatchPercentage);
    }

    
    /// Match a specific resume against all job descriptions
    
    public async Task<IEnumerable<MatchingResultDto>> MatchResumeToAllJobsAsync(int resumeId)
    {
        var jobs = await _unitOfWork.JobDescriptions.GetAllAsync();
        var results = new List<MatchingResultDto>();

        foreach (var job in jobs)
        {
            try
            {
                var result = await MatchResumeToJobAsync(resumeId, job.Id);
                results.Add(result);
            }
            catch (Exception)
            {
                // Skip jobs that fail matching
                continue;
            }
        }

        // Sort by match percentage descending
        return results.OrderByDescending(r => r.MatchPercentage);
    }

    
    /// Get existing matching result if available
    
    public async Task<MatchingResultDto?> GetMatchingResultAsync(int resumeId, int jobDescriptionId)
    {
        var matchingResult = await _unitOfWork.MatchingResults.FirstOrDefaultAsync(
            mr => mr.ResumeId == resumeId && mr.JobDescriptionId == jobDescriptionId);

        if (matchingResult == null)
            return null;

        // Load related data
        var resume = await _unitOfWork.Resumes.GetResumeWithSkillsAsync(resumeId);
        var job = await _unitOfWork.JobDescriptions.GetJobWithSkillsAsync(jobDescriptionId);

        if (resume == null || job == null)
            return null;

        // Calculate skill lists
        var resumeSkillNames = resume.ResumeSkills.Select(rs => rs.Skill.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var jobSkillNames = job.JobSkills.Select(js => js.Skill.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        var matchingSkills = resumeSkillNames.Intersect(jobSkillNames, StringComparer.OrdinalIgnoreCase).ToList();
        var missingSkills = jobSkillNames.Except(resumeSkillNames, StringComparer.OrdinalIgnoreCase).ToList();

        return new MatchingResultDto
        {
            Id = matchingResult.Id,
            ResumeId = resumeId,
            ResumeFileName = resume.FileName,
            JobDescriptionId = jobDescriptionId,
            JobTitle = job.JobTitle,
            Company = job.Company,
            MatchPercentage = matchingResult.MatchPercentage,
            CosineSimilarityScore = matchingResult.CosineSimilarityScore,
            MatchingSkillsCount = matchingResult.MatchingSkillsCount,
            TotalJobSkillsCount = matchingResult.TotalJobSkillsCount,
            SkillMatchScore = matchingResult.SkillMatchScore,
            MatchedAt = matchingResult.MatchedAt,
            MatchingSkills = matchingSkills,
            MissingSkills = missingSkills
        };
    }
}

