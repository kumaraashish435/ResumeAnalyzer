namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// Skill Extraction Service Interface
/// Defines operations for extracting skills from resume text
/// Identifies candidate skills to match against job requirements
/// Uses skill dictionary matching and text analysis to find skills in resume

public interface ISkillExtractionService
{
    
    /// Extract skills from preprocessed resume text
    
    /// <param name="preprocessedText">Cleaned resume text</param>
    /// <param name="skillDictionary">Dictionary of known skills to match against</param>
    /// <returns>Dictionary mapping skill names to confidence scores (0-1)</returns>
    Dictionary<string, double> ExtractSkills(string preprocessedText, IEnumerable<string> skillDictionary);
    
    
    /// Extract skills with case-insensitive fuzzy matching
    
    /// <param name="preprocessedText">Cleaned resume text</param>
    /// <param name="skillDictionary">Dictionary of known skills</param>
    /// <param name="fuzzyThreshold">Similarity threshold for fuzzy matching (0-1)</param>
    /// <returns>Dictionary mapping skill names to confidence scores</returns>
    Dictionary<string, double> ExtractSkillsWithFuzzyMatching(
        string preprocessedText, 
        IEnumerable<string> skillDictionary, 
        double fuzzyThreshold = 0.8);
}

