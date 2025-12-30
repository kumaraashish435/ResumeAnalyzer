using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Application.Services;

namespace ResumeAnalyzer.Web.Controllers;

/// <summary>
/// Job Description Controller
/// What: Handles HTTP requests related to job description management
/// Why: MVC controller for creating and managing job descriptions
/// How: Provides CRUD operations for job descriptions through web interface
/// </summary>
public class JobDescriptionController : Controller
{
    private readonly IJobDescriptionService _jobDescriptionService;
    private readonly ILogger<JobDescriptionController> _logger;

    public JobDescriptionController(
        IJobDescriptionService jobDescriptionService,
        ILogger<JobDescriptionController> logger)
    {
        _jobDescriptionService = jobDescriptionService;
        _logger = logger;
    }

    /// <summary>
    /// Display list of all job descriptions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var jobs = await _jobDescriptionService.GetAllJobDescriptionsAsync();
            return View(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job descriptions");
            return View(new List<JobDescriptionDto>());
        }
    }

    /// <summary>
    /// Display job description creation form
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handle job description creation
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateJobDescriptionDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createDto);
        }

        try
        {
            var jobDto = await _jobDescriptionService.CreateJobDescriptionAsync(createDto);
            TempData["SuccessMessage"] = $"Job description '{jobDto.JobTitle}' created successfully!";
            return RedirectToAction(nameof(Details), new { id = jobDto.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred while creating the job description. Please try again.");
            _logger.LogError(ex, "Error creating job description");
        }

        return View(createDto);
    }

    /// <summary>
    /// Display job description details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var job = await _jobDescriptionService.GetJobDescriptionByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job description details");
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a job description
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            bool deleted = await _jobDescriptionService.DeleteJobDescriptionAsync(id);
            if (deleted)
            {
                TempData["SuccessMessage"] = "Job description deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Job description not found.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job description");
            TempData["ErrorMessage"] = "An error occurred while deleting the job description.";
        }

        return RedirectToAction(nameof(Index));
    }
}

