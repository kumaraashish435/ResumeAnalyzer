namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// Text Preprocessing Service Interface
/// Defines operations for cleaning and preprocessing text before NLP analysis
/// Clean text improves accuracy of skill extraction and similarity matching
/// Performs lowercasing, tokenization, stop-word removal, and normalization

public interface ITextPreprocessingService
{
    
    /// Preprocess text by cleaning, tokenizing, and removing stop words
    
    /// <param name="text">Raw text input</param>
    /// <returns>Preprocessed text tokens as a list</returns>
    List<string> PreprocessText(string text);
    
    
    /// Preprocess text and return as a single cleaned string
    
    /// <param name="text">Raw text input</param>
    /// <returns>Preprocessed text as a single string</returns>
    string PreprocessTextToString(string text);
}

