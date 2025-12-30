using System.Text.RegularExpressions;

namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// Skill Extraction Service Implementation
/// Extracts skills from resume text using dictionary-based matching
/// Identifies candidate competencies to compare against job requirements
/// Matches resume text against a skill dictionary using exact and partial matching
/// 
/// Skill Extraction Strategy:
/// 1. Exact matching: Find exact skill names in the text
/// 2. Partial matching: Find skill names that appear as part of words (e.g., "C#" in "C# Developer")
/// 3. Confidence scoring: Assign higher confidence to exact matches, lower to partial matches
/// 
/// Future Enhancement: Can be extended with:
/// - Machine learning models for skill recognition
/// - Named Entity Recognition (NER) for skill detection
/// - Context-aware skill extraction

public class SkillExtractionService : ISkillExtractionService
{
    
    /// Extract skills using exact and partial matching
    
    public Dictionary<string, double> ExtractSkills(string preprocessedText, IEnumerable<string> skillDictionary)
    {
        if (string.IsNullOrWhiteSpace(preprocessedText) || skillDictionary == null)
            return new Dictionary<string, double>();

        var extractedSkills = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var textLower = preprocessedText.ToLowerInvariant();

        foreach (string skill in skillDictionary)
        {
            if (string.IsNullOrWhiteSpace(skill))
                continue;

            string skillLower = skill.ToLowerInvariant();
            double confidence = 0.0;

            // Exact match: Skill appears as a complete word (highest confidence)
            // Using word boundaries to ensure we match complete words
            string exactPattern = @"\b" + Regex.Escape(skillLower) + @"\b";
            if (Regex.IsMatch(textLower, exactPattern, RegexOptions.IgnoreCase))
            {
                confidence = 1.0; // Full confidence for exact matches
            }
            // Partial match: Skill appears as part of a word or phrase (lower confidence)
            else if (textLower.Contains(skillLower))
            {
                confidence = 0.7; // Lower confidence for partial matches
            }

            // Only add skills with confidence above threshold
            if (confidence > 0)
            {
                extractedSkills[skill] = confidence;
            }
        }

        return extractedSkills;
    }

    
    /// Extract skills with fuzzy matching for typos and variations
    /// Uses Levenshtein distance for approximate string matching
    
    public Dictionary<string, double> ExtractSkillsWithFuzzyMatching(
        string preprocessedText, 
        IEnumerable<string> skillDictionary, 
        double fuzzyThreshold = 0.8)
    {
        if (string.IsNullOrWhiteSpace(preprocessedText) || skillDictionary == null)
            return new Dictionary<string, double>();

        var extractedSkills = ExtractSkills(preprocessedText, skillDictionary);
        var textLower = preprocessedText.ToLowerInvariant();

        // For skills not found with exact/partial matching, try fuzzy matching
        foreach (string skill in skillDictionary)
        {
            if (extractedSkills.ContainsKey(skill))
                continue; // Already found with higher confidence

            string skillLower = skill.ToLowerInvariant();
            double bestSimilarity = 0.0;

            // Split text into words and check similarity with each word
            var words = textLower.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string word in words)
            {
                double similarity = CalculateLevenshteinSimilarity(skillLower, word);
                if (similarity > bestSimilarity)
                    bestSimilarity = similarity;
            }

            // If similarity is above threshold, add with lower confidence
            if (bestSimilarity >= fuzzyThreshold)
            {
                extractedSkills[skill] = bestSimilarity * 0.6; // Fuzzy matches have lower confidence
            }
        }

        return extractedSkills;
    }

    
    /// Calculate similarity between two strings using Levenshtein distance
    /// Returns a value between 0 (completely different) and 1 (identical)
    
    private double CalculateLevenshteinSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0.0;

        if (s1 == s2)
            return 1.0;

        int maxLength = Math.Max(s1.Length, s2.Length);
        if (maxLength == 0)
            return 1.0;

        int distance = LevenshteinDistance(s1, s2);
        return 1.0 - (double)distance / maxLength;
    }

    
    /// Calculate Levenshtein distance (edit distance) between two strings
    /// The minimum number of single-character edits needed to change one word into another
    
    private int LevenshteinDistance(string s1, string s2)
    {
        int[,] distance = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
            distance[i, 0] = i;

        for (int j = 0; j <= s2.Length; j++)
            distance[0, j] = j;

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = (s2[j - 1] == s1[i - 1]) ? 0 : 1;
                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        return distance[s1.Length, s2.Length];
    }
}

