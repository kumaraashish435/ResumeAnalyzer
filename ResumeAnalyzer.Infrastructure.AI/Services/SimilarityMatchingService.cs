using System.Linq;

namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// Similarity Matching Service Implementation
/// Calculates similarity between resume and job description using TF-IDF and cosine similarity
/// TF-IDF (Term Frequency-Inverse Document Frequency) converts text to numerical vectors
///      that capture the importance of words. Cosine similarity measures the angle between vectors,
///      giving us a similarity score independent of document length.
/// 
/// How It Works:
/// 1. TF-IDF Vectorization: Converts text documents to numerical vectors
///    - TF (Term Frequency): How often a word appears in a document (normalized by document length)
///    - IDF (Inverse Document Frequency): How rare/common a word is across all documents
///    - TF-IDF = TF × IDF: Gives higher weight to words that are frequent in the document but rare overall
/// 
/// 2. Cosine Similarity: Measures the cosine of the angle between two vectors
///    - Formula: cosine(θ) = (A · B) / (||A|| × ||B||)
///    - Where A and B are TF-IDF vectors
///    - Result: Value between 0 (orthogonal, no similarity) and 1 (identical direction)
/// 
/// Why TF-IDF is Used:
/// - Captures semantic importance: Important terms get higher weights
/// - Handles document length differences: Normalizes by document size
/// - Reduces noise: Common words (like "the", "and") get low weights automatically
/// - Works well for resume-job matching: Highlights technical skills and keywords
/// 
/// Mathematical Formula for Cosine Similarity:
/// cosine_similarity(A, B) = Σ(A_i × B_i) / (√(Σ(A_i²)) × √(Σ(B_i²)))
/// 
/// Where:
/// - A_i, B_i are the TF-IDF values for term i in documents A and B
/// - The numerator is the dot product of vectors A and B
/// - The denominator is the product of the magnitudes (Euclidean norms) of A and B

public class SimilarityMatchingService : ISimilarityMatchingService
{
    
    /// Calculate cosine similarity between two preprocessed text documents
    
    public double CalculateCosineSimilarity(string resumeText, string jobText)
    {
        if (string.IsNullOrWhiteSpace(resumeText) || string.IsNullOrWhiteSpace(jobText))
            return 0.0;

        // Step 1: Calculate TF-IDF vectors for both documents
        var (resumeVector, jobVector, vocabulary) = CalculateTfIdfVectors(resumeText, jobText);

        if (vocabulary.Count == 0)
            return 0.0;

        // Step 2: Calculate dot product of the two vectors
        double dotProduct = 0.0;
        foreach (string term in vocabulary)
        {
            double resumeValue = resumeVector.GetValueOrDefault(term, 0.0);
            double jobValue = jobVector.GetValueOrDefault(term, 0.0);
            dotProduct += resumeValue * jobValue;
        }

        // Step 3: Calculate magnitudes (Euclidean norms) of both vectors
        double resumeMagnitude = Math.Sqrt(resumeVector.Values.Sum(x => x * x));
        double jobMagnitude = Math.Sqrt(jobVector.Values.Sum(x => x * x));

        // Step 4: Calculate cosine similarity
        if (resumeMagnitude == 0.0 || jobMagnitude == 0.0)
            return 0.0;

        double cosineSimilarity = dotProduct / (resumeMagnitude * jobMagnitude);

        // Ensure result is between 0 and 1
        return Math.Max(0.0, Math.Min(1.0, cosineSimilarity));
    }

    
    /// Calculate comprehensive match percentage combining cosine similarity and skill matching
    
    public double CalculateMatchPercentage(
        string resumeText, 
        string jobText, 
        int matchingSkills, 
        int totalJobSkills)
    {
        // Step 1: Calculate cosine similarity (0 to 1)
        double cosineSimilarity = CalculateCosineSimilarity(resumeText, jobText);

        // Step 2: Calculate skill match score
        double skillMatchScore = totalJobSkills > 0 
            ? (double)matchingSkills / totalJobSkills 
            : 0.0;

        // Step 3: Combine both scores with weighted average
        // Weight: 60% cosine similarity (semantic match) + 40% skill overlap (exact match)
        // This balances overall content similarity with specific skill requirements
        double combinedScore = (cosineSimilarity * 0.6) + (skillMatchScore * 0.4);

        // Convert to percentage (0 to 100)
        return Math.Round(combinedScore * 100, 2);
    }

    
    /// Calculate TF-IDF vectors for resume and job description
    /// In a production system with multiple documents, IDF would use a corpus
    /// Here we use a simplified version for two-document comparison
    
    public (Dictionary<string, double> resumeVector, Dictionary<string, double> jobVector, HashSet<string> vocabulary) 
        CalculateTfIdfVectors(string resumeText, string jobText)
    {
        // Tokenize both documents
        var resumeTokens = Tokenize(resumeText);
        var jobTokens = Tokenize(jobText);

        // Create vocabulary (unique terms from both documents)
        var vocabulary = new HashSet<string>(resumeTokens);
        vocabulary.UnionWith(jobTokens);

        // Calculate Term Frequency (TF) for both documents
        var resumeTf = CalculateTermFrequency(resumeTokens);
        var jobTf = CalculateTermFrequency(jobTokens);

        // For two-document scenario, simplified IDF calculation
        // IDF(t) = log(2 / (1 + number of documents containing t))
        var idf = new Dictionary<string, double>();
        foreach (string term in vocabulary)
        {
            int docCount = 0;
            if (resumeTf.ContainsKey(term)) docCount++;
            if (jobTf.ContainsKey(term)) docCount++;
            
            // IDF formula: log(2 / (1 + docCount))
            // +1 in denominator to avoid division by zero
            idf[term] = Math.Log(2.0 / (1.0 + docCount));
        }

        // Calculate TF-IDF vectors
        var resumeVector = new Dictionary<string, double>();
        var jobVector = new Dictionary<string, double>();

        foreach (string term in vocabulary)
        {
            double resumeTfValue = resumeTf.GetValueOrDefault(term, 0.0);
            double jobTfValue = jobTf.GetValueOrDefault(term, 0.0);
            
            resumeVector[term] = resumeTfValue * idf[term];
            jobVector[term] = jobTfValue * idf[term];
        }

        return (resumeVector, jobVector, vocabulary);
    }

    
    /// Tokenize text into words (simple whitespace-based tokenization)
    
    private List<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    
    /// Calculate Term Frequency (TF) for tokens
    /// TF(t) = (Number of times term t appears in document) / (Total number of terms in document)
    
    private Dictionary<string, double> CalculateTermFrequency(List<string> tokens)
    {
        var tf = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        
        if (tokens.Count == 0)
            return tf;

        // Count occurrences of each term
        var termCounts = tokens
            .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        // Calculate TF: count / total terms
        int totalTerms = tokens.Count;
        foreach (var kvp in termCounts)
        {
            tf[kvp.Key] = (double)kvp.Value / totalTerms;
        }

        return tf;
    }
}

