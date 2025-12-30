namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// PDF Extraction Service Interface
/// Defines operations for extracting text from PDF resume files
/// Abstracts PDF parsing logic, making it testable and replaceable
/// Takes a PDF file path and returns extracted text content

public interface IPdfExtractionService
{
    
    /// Extract text content from a PDF file
    
    /// <param name="pdfFilePath">Full path to the PDF file</param>
    /// <returns>Extracted text content as a string</returns>
    Task<string> ExtractTextFromPdfAsync(string pdfFilePath);
    
    
    /// Extract text content from a PDF file stream
    
    /// <param name="pdfStream">Stream containing PDF data</param>
    /// <returns>Extracted text content as a string</returns>
    Task<string> ExtractTextFromPdfStreamAsync(Stream pdfStream);
}

