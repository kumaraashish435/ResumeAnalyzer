namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// Similarity Matching Service Interface
/// Defines operations for calculating similarity between resume and job description
/// Core matching engine that determines how well a resume matches a job
/// Uses TF-IDF vectorization and cosine similarity to compute match scores

public interface ISimilarityMatchingService
{
    
    /// Calculate cosine similarity between two text documents using TF-IDF
    
    /// <param name="resumeText">Preprocessed resume text</param>
    /// <param name="jobText">Preprocessed job description text</param>
    /// <returns>Cosine similarity score between 0 (no similarity) and 1 (identical)</returns>
    double CalculateCosineSimilarity(string resumeText, string jobText);
    
    
    /// Calculate comprehensive match percentage combining multiple factors
    
    /// <param name="resumeText">Preprocessed resume text</param>
    /// <param name="jobText">Preprocessed job description text</param>
    /// <param name="matchingSkills">Number of matching skills</param>
    /// <param name="totalJobSkills">Total number of required job skills</param>
    /// <returns>Match percentage between 0 and 100</returns>
    double CalculateMatchPercentage(
        string resumeText, 
        string jobText, 
        int matchingSkills, 
        int totalJobSkills);
    
    
    /// Calculate TF-IDF vectors for two documents
    
    /// <returns>Tuple of (resumeVector, jobVector, vocabulary)</returns>
    (Dictionary<string, double> resumeVector, Dictionary<string, double> jobVector, HashSet<string> vocabulary) 
        CalculateTfIdfVectors(string resumeText, string jobText);
}

