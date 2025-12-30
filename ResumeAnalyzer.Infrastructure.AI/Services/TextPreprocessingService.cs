using System.Text;
using System.Text.RegularExpressions;

namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// Text Preprocessing Service Implementation
/// Cleans and normalizes text for NLP processing
/// Raw PDF text contains noise (special characters, formatting, stop words) that reduces matching accuracy
/// Applies multiple cleaning steps: lowercasing, tokenization, stop-word removal, normalization
/// 
/// NLP Preprocessing Steps Explained:
/// 1. Lowercasing: Converts all text to lowercase for case-insensitive matching
/// 2. Tokenization: Splits text into individual words/tokens
/// 3. Stop-word removal: Removes common words (the, is, a, an) that don't carry meaning
/// 4. Normalization: Removes special characters, extra spaces, and normalizes punctuation

public class TextPreprocessingService : ITextPreprocessingService
{
    // Common English stop words that don't add semantic value
    // These words appear frequently but don't help in skill/resume matching
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
        "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
        "to", "was", "will", "with", "the", "this", "but", "they", "have",
        "had", "what", "said", "each", "which", "their", "time", "if",
        "up", "out", "many", "then", "them", "these", "so", "some", "her",
        "would", "make", "like", "into", "him", "has", "two", "more",
        "very", "after", "words", "long", "than", "first", "been", "call",
        "who", "oil", "sit", "now", "find", "down", "day", "did", "get",
        "come", "made", "may", "part", "i", "we", "you", "she", "do",
        "can", "could", "should", "would", "may", "might", "must"
    };

    
    /// Preprocess text and return as a list of cleaned tokens
    
    public List<string> PreprocessText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        // Step 1: Convert to lowercase for case-insensitive matching
        string lowerText = text.ToLowerInvariant();

        // Step 2: Remove special characters, keep only letters, numbers, and spaces
        // This removes PDF artifacts, special symbols, and formatting characters
        string cleanedText = Regex.Replace(lowerText, @"[^a-z0-9\s]", " ", RegexOptions.Compiled);

        // Step 3: Replace multiple whitespaces with single space
        cleanedText = Regex.Replace(cleanedText, @"\s+", " ", RegexOptions.Compiled);

        // Step 4: Split into tokens (words)
        var tokens = cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        // Step 5: Remove stop words (common words that don't carry semantic meaning)
        var filteredTokens = tokens
            .Where(token => !StopWords.Contains(token) && token.Length > 1) // Also filter single characters
            .ToList();

        return filteredTokens;
    }

    
    /// Preprocess text and return as a single cleaned string
    
    public string PreprocessTextToString(string text)
    {
        var tokens = PreprocessText(text);
        return string.Join(" ", tokens);
    }
}

