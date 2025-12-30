using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Application.Services;

namespace ResumeAnalyzer.Web.Controllers;


/// Resume Controller
/// Handles HTTP requests related to resume operations (upload, view, delete)
/// MVC controller that coordinates between user interface and business logic
/// Receives form submissions, calls application services, returns views or JSON

public class ResumeController : Controller
{
    private readonly IResumeService _resumeService;
    private readonly ILogger<ResumeController> _logger;
    private readonly IWebHostEnvironment _environment;

    // Uploads folder path configuration key
    private const string UploadsFolder = "Uploads/Resumes";

    public ResumeController(
        IResumeService resumeService, 
        ILogger<ResumeController> logger,
        IWebHostEnvironment environment)
    {
        _resumeService = resumeService;
        _logger = logger;
        _environment = environment;
    }

    
    /// Display resume upload form
    
    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    
    /// Handle resume file upload
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(ResumeUploadDto uploadDto)
    {
        if (!ModelState.IsValid)
        {
            return View(uploadDto);
        }

        try
        {
            // Get uploads folder path
            string uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, UploadsFolder);

            // Upload and process resume
            var resumeDto = await _resumeService.UploadResumeAsync(uploadDto, uploadsPath);

            TempData["SuccessMessage"] = $"Resume '{resumeDto.FileName}' uploaded and processed successfully!";
            return RedirectToAction(nameof(Details), new { id = resumeDto.Id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            _logger.LogWarning(ex, "Invalid file upload attempt");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred while uploading the resume. Please try again.");
            _logger.LogError(ex, "Error uploading resume");
        }

        return View(uploadDto);
    }

    
    /// Display list of resumes for a user
    
    [HttpGet]
    public async Task<IActionResult> Index(int userId = 1) // Default userId for demo
    {
        try
        {
            var resumes = await _resumeService.GetResumesByUserIdAsync(userId);
            return View(resumes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving resumes");
            return View(new List<ResumeDto>());
        }
    }

    
    /// Display resume details
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var resume = await _resumeService.GetResumeByIdAsync(id);
            if (resume == null)
            {
                return NotFound();
            }
            return View(resume);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving resume details");
            return NotFound();
        }
    }

    
    /// Delete a resume
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            bool deleted = await _resumeService.DeleteResumeAsync(id);
            if (deleted)
            {
                TempData["SuccessMessage"] = "Resume deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Resume not found.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting resume");
            TempData["ErrorMessage"] = "An error occurred while deleting the resume.";
        }

        return RedirectToAction(nameof(Index));
    }
}

