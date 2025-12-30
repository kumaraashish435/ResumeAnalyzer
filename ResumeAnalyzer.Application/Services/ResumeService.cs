using System.Linq;
using Microsoft.AspNetCore.Http;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Infrastructure.AI.Services;
using ResumeAnalyzer.Infrastructure.Data.Repositories;

namespace ResumeAnalyzer.Application.Services;


/// Resume Service Implementation
/// Implements business logic for resume operations
/// Orchestrates file handling, PDF extraction, text processing, and skill extraction
/// Coordinates between repositories, AI services, and file system operations
/// 
/// Best Practices Used:
/// - Separation of concerns: Business logic separate from data access
/// - Error handling: Validates file types and handles exceptions
/// - Async/await: Non-blocking I/O operations
/// - File validation: Checks file extension and MIME type

public class ResumeService : IResumeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfExtractionService _pdfExtractionService;
    private readonly ITextPreprocessingService _textPreprocessingService;
    private readonly ISkillExtractionService _skillExtractionService;

    public ResumeService(
        IUnitOfWork unitOfWork,
        IPdfExtractionService pdfExtractionService,
        ITextPreprocessingService textPreprocessingService,
        ISkillExtractionService skillExtractionService)
    {
        _unitOfWork = unitOfWork;
        _pdfExtractionService = pdfExtractionService;
        _textPreprocessingService = textPreprocessingService;
        _skillExtractionService = skillExtractionService;
    }

    
    /// Upload and process a resume PDF file
    
    public async Task<ResumeDto> UploadResumeAsync(ResumeUploadDto uploadDto, string uploadsFolderPath)
    {
        // Validation
        if (uploadDto.File == null)
            throw new ArgumentNullException(nameof(uploadDto.File), "File cannot be null");

        // Validate file type
        ValidatePdfFile(uploadDto.File);

        // Ensure uploads directory exists
        if (!Directory.Exists(uploadsFolderPath))
            Directory.CreateDirectory(uploadsFolderPath);

        // Generate unique filename to prevent conflicts
        string uniqueFileName = $"{Guid.NewGuid()}_{uploadDto.File.FileName}";
        string filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await uploadDto.File.CopyToAsync(stream);
        }

        // Extract text from PDF
        string extractedText;
        try
        {
            extractedText = await _pdfExtractionService.ExtractTextFromPdfAsync(filePath);
        }
        catch (Exception ex)
        {
            // Clean up file if extraction fails
            if (File.Exists(filePath))
                File.Delete(filePath);
            throw new InvalidOperationException($"Failed to extract text from PDF: {ex.Message}", ex);
        }

        // Preprocess extracted text
        string preprocessedText = _textPreprocessingService.PreprocessTextToString(extractedText);

        // Get skill dictionary from database
        var allSkills = await _unitOfWork.Skills.GetAllAsync();
        var skillDictionary = allSkills.Select(s => s.Name).ToList();

        // Extract skills from resume
        var extractedSkills = _skillExtractionService.ExtractSkills(preprocessedText, skillDictionary);

        // Create Resume entity
        var resume = new Resume
        {
            UserId = uploadDto.UserId,
            FileName = uploadDto.File.FileName,
            FilePath = filePath,
            ExtractedText = extractedText, // Store original extracted text
            FileSize = uploadDto.File.Length,
            UploadedAt = DateTime.UtcNow,
            ProcessedAt = DateTime.UtcNow
        };

        // Save resume to database
        await _unitOfWork.Resumes.AddAsync(resume);
        await _unitOfWork.SaveChangesAsync();

        // Save extracted skills
        foreach (var skillKvp in extractedSkills)
        {
            var skill = allSkills.FirstOrDefault(s => s.Name.Equals(skillKvp.Key, StringComparison.OrdinalIgnoreCase));
            if (skill != null)
            {
                var resumeSkill = new ResumeSkill
                {
                    ResumeId = resume.Id,
                    SkillId = skill.Id,
                    ConfidenceScore = skillKvp.Value,
                    ExtractedAt = DateTime.UtcNow
                };
                await _unitOfWork.ResumeSkills.AddAsync(resumeSkill);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        // Map to DTO and return
        return MapToDto(resume, extractedSkills.Keys.ToList());
    }

    
    /// Get all resumes for a user
    
    public async Task<IEnumerable<ResumeDto>> GetResumesByUserIdAsync(int userId)
    {
        var resumes = await _unitOfWork.Resumes.GetResumesByUserIdAsync(userId);
        
        return resumes.Select(r => new ResumeDto
        {
            Id = r.Id,
            FileName = r.FileName,
            FileSize = r.FileSize,
            UploadedAt = r.UploadedAt,
            ProcessedAt = r.ProcessedAt,
            UserName = r.User?.Name ?? "Unknown",
            Skills = r.ResumeSkills.Select(rs => rs.Skill.Name).ToList()
        });
    }

    
    /// Get a specific resume by ID
    
    public async Task<ResumeDto?> GetResumeByIdAsync(int resumeId)
    {
        var resume = await _unitOfWork.Resumes.GetResumeWithDetailsAsync(resumeId);
        if (resume == null)
            return null;

        return MapToDto(resume, resume.ResumeSkills.Select(rs => rs.Skill.Name).ToList());
    }

    
    /// Delete a resume
    
    public async Task<bool> DeleteResumeAsync(int resumeId)
    {
        var resume = await _unitOfWork.Resumes.GetByIdAsync(resumeId);
        if (resume == null)
            return false;

        // Delete physical file
        if (File.Exists(resume.FilePath))
        {
            try
            {
                File.Delete(resume.FilePath);
            }
            catch
            {
                // Log error but continue with database deletion
            }
        }

        // Delete from database (cascade will handle related records)
        await _unitOfWork.Resumes.DeleteAsync(resume);
        return true;
    }

    
    /// Validate that uploaded file is a PDF
    
    private void ValidatePdfFile(IFormFile file)
    {
        // Check file extension
        var allowedExtensions = new[] { ".pdf" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Only PDF files are allowed", nameof(file));

        // Check MIME type
        var allowedMimeTypes = new[] { "application/pdf" };
        if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new ArgumentException("Invalid file type. Only PDF files are allowed", nameof(file));

        // Check file size (e.g., max 10MB)
        const long maxFileSize = 10 * 1024 * 1024; // 10MB
        if (file.Length > maxFileSize)
            throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB", nameof(file));

        if (file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));
    }

    
    /// Map Resume entity to ResumeDto
    
    private ResumeDto MapToDto(Resume resume, List<string> skills)
    {
        return new ResumeDto
        {
            Id = resume.Id,
            FileName = resume.FileName,
            FileSize = resume.FileSize,
            UploadedAt = resume.UploadedAt,
            ProcessedAt = resume.ProcessedAt,
            UserName = resume.User?.Name ?? "Unknown",
            Skills = skills
        };
    }
}

