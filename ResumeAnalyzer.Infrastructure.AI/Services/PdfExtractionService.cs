using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf.Content;
using PdfSharpCore.Pdf.Content.Objects;
using System.Text;

namespace ResumeAnalyzer.Infrastructure.AI.Services;


/// PDF Extraction Service Implementation
/// Extracts text content from PDF resume files using PdfSharpCore library
/// PdfSharpCore is a cross-platform .NET library that can parse PDF files and extract text
/// Opens PDF file, iterates through pages, extracts text content from each page using content stream parsing
/// 
/// Best Practices Used:
/// - Using PdfSharpCore (cross-platform .NET library for PDF processing)
/// - Proper disposal of resources using using statements
/// - Error handling for corrupted or invalid PDF files
/// - Recursive content object parsing for text extraction

public class PdfExtractionService : IPdfExtractionService
{
    
    /// Extract text from a PDF file stored on disk
    
    public async Task<string> ExtractTextFromPdfAsync(string pdfFilePath)
    {
        if (string.IsNullOrWhiteSpace(pdfFilePath))
            throw new ArgumentException("PDF file path cannot be null or empty", nameof(pdfFilePath));
        
        if (!File.Exists(pdfFilePath))
            throw new FileNotFoundException($"PDF file not found: {pdfFilePath}");

        return await Task.Run(() =>
        {
            var extractedText = new StringBuilder();

            try
            {
                // PdfSharpCore opens and parses the PDF file
                using var document = PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.ReadOnly);
                
                // Iterate through all pages in the PDF
                foreach (var page in document.Pages)
                {
                    // Extract text from the current page
                    string pageText = ExtractTextFromPage(page);
                    
                    // Add page text with newline separator
                    extractedText.AppendLine(pageText);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error extracting text from PDF: {ex.Message}", ex);
            }

            return extractedText.ToString().Trim();
        });
    }

    
    /// Extract text from a PDF file stream (for uploaded files)
    /// Note: PdfSharpCore requires a file path, so we save stream to temp file first
    
    public async Task<string> ExtractTextFromPdfStreamAsync(Stream pdfStream)
    {
        if (pdfStream == null)
            throw new ArgumentNullException(nameof(pdfStream));
        
        if (!pdfStream.CanRead)
            throw new ArgumentException("Stream must be readable", nameof(pdfStream));

        // Save stream to temporary file since PdfSharpCore requires a file path
        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");
        
        try
        {
            // Reset stream position to beginning
            pdfStream.Position = 0;
            
            // Save stream to temporary file
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                await pdfStream.CopyToAsync(fileStream);
            }
            
            // Extract text using file path method
            return await ExtractTextFromPdfAsync(tempFilePath);
        }
        finally
        {
            // Clean up temporary file
            if (File.Exists(tempFilePath))
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
            
            // Reset stream position after reading
            pdfStream.Position = 0;
        }
    }

    
    /// Extract text from a PDF page using PdfSharpCore ContentReader
    
    private string ExtractTextFromPage(PdfPage page)
    {
        var textBuilder = new StringBuilder();
        
        try
        {
            // Get the content stream for the page
            var content = ContentReader.ReadContent(page);
            
            // Extract text from content objects recursively
            ExtractTextFromContentObjects(content, textBuilder);
        }
        catch
        {
            // If content extraction fails, return empty string
            // Some PDFs may have non-text content, encrypted pages, or complex structures
        }

        return textBuilder.ToString();
    }

    
    /// Recursively extract text from PDF content objects
    /// Handles CArray, CSequence, COperator, and CString objects
    
    private void ExtractTextFromContentObjects(CObject obj, StringBuilder textBuilder)
    {
        if (obj == null)
            return;

        switch (obj)
        {
            case CArray array:
                // Process each item in the array
                foreach (var item in array)
                {
                    ExtractTextFromContentObjects(item, textBuilder);
                }
                break;

            case CSequence sequence:
                // Process each item in the sequence
                foreach (var item in sequence)
                {
                    ExtractTextFromContentObjects(item, textBuilder);
                }
                break;

            case COperator op:
                // Text operators: Tj (show text) and TJ (show text with positioning)
                // These operators indicate where text should be displayed
                if (op.OpCode.OpCodeName == OpCodeName.Tj || op.OpCode.OpCodeName == OpCodeName.TJ)
                {
                    // Extract text from operands
                    foreach (var operand in op.Operands)
                    {
                        ExtractTextFromContentObjects(operand, textBuilder);
                    }
                    textBuilder.Append(" "); // Add space after text operator
                }
                break;

            case CString str:
                // Direct string content - add to text builder
                textBuilder.Append(str.Value);
                break;

            case CInteger integer:
            case CReal real:
            case CName name:
            case CComment comment:
                // These types don't contain extractable text
                break;
        }
    }
}
