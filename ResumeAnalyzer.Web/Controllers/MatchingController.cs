using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Application.Services;

namespace ResumeAnalyzer.Web.Controllers;


/// Matching Controller
/// Handles HTTP requests for resume-job matching operations
/// Provides interface for users to match resumes against job descriptions
/// Displays matching dashboard and results with match scores

public class MatchingController : Controller
{
    private readonly IMatchingService _matchingService;
    private readonly IResumeService _resumeService;
    private readonly IJobDescriptionService _jobDescriptionService;
    private readonly ILogger<MatchingController> _logger;

    public MatchingController(
        IMatchingService matchingService,
        IResumeService resumeService,
        IJobDescriptionService jobDescriptionService,
        ILogger<MatchingController> logger)
    {
        _matchingService = matchingService;
        _resumeService = resumeService;
        _jobDescriptionService = jobDescriptionService;
        _logger = logger;
    }

    
    /// Display matching dashboard
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var resumes = await _resumeService.GetResumesByUserIdAsync(1); // Default userId for demo
            var jobs = await _jobDescriptionService.GetAllJobDescriptionsAsync();

            ViewBag.Resumes = resumes;
            ViewBag.Jobs = jobs;

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading matching dashboard");
            return View();
        }
    }

    
    /// Match a specific resume to a specific job
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Match(int resumeId, int jobDescriptionId)
    {
        try
        {
            var result = await _matchingService.MatchResumeToJobAsync(resumeId, jobDescriptionId);
            return RedirectToAction(nameof(Result), new { resumeId, jobDescriptionId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error matching resume to job");
            TempData["ErrorMessage"] = "An error occurred while matching. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    
    /// Display matching result
    
    [HttpGet]
    public async Task<IActionResult> Result(int resumeId, int jobDescriptionId)
    {
        try
        {
            var result = await _matchingService.GetMatchingResultAsync(resumeId, jobDescriptionId);
            if (result == null)
            {
                // If result doesn't exist, perform matching now
                result = await _matchingService.MatchResumeToJobAsync(resumeId, jobDescriptionId);
            }
            return View(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving matching result");
            TempData["ErrorMessage"] = "Error retrieving matching result.";
            return RedirectToAction(nameof(Index));
        }
    }

    
    /// Match all resumes to a specific job
    
    [HttpGet]
    public async Task<IActionResult> MatchAllToJob(int jobDescriptionId)
    {
        try
        {
            var results = await _matchingService.MatchAllResumesToJobAsync(jobDescriptionId);
            ViewBag.JobId = jobDescriptionId;
            return View("MatchResults", results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error matching all resumes to job");
            TempData["ErrorMessage"] = "An error occurred while matching. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    
    /// Match a specific resume to all jobs
    
    [HttpGet]
    public async Task<IActionResult> MatchResumeToAll(int resumeId)
    {
        try
        {
            var results = await _matchingService.MatchResumeToAllJobsAsync(resumeId);
            ViewBag.ResumeId = resumeId;
            return View("MatchResults", results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error matching resume to all jobs");
            TempData["ErrorMessage"] = "An error occurred while matching. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }
}

