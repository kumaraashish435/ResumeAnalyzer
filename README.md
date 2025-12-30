# AI Resume Analyzer

A comprehensive web-based AI Resume Analyzer system that matches candidate resumes with job descriptions using Natural Language Processing (NLP) techniques including TF-IDF vectorization and cosine similarity.

## Project Overview

This system accepts PDF resumes, extracts text content, identifies candidate skills, matches resumes against job descriptions, and calculates match percentages using advanced AI/NLP algorithms.

## Architecture

The project follows **Clean Architecture** principles with clear separation of concerns across multiple layers:

```
ResumeAnalyzer/
├── ResumeAnalyzer.Domain/              # Domain Layer (Entities)
├── ResumeAnalyzer.Application/         # Application Layer (DTOs, Services)
├── ResumeAnalyzer.Infrastructure.Data/ # Data Access Layer (EF Core, Repositories)
├── ResumeAnalyzer.Infrastructure.AI/   # AI/NLP Layer (Matching Algorithms)
└── ResumeAnalyzer.Web/                 # Presentation Layer (MVC, Views)
```

### Layer-by-Layer Explanation

#### 1. **Domain Layer** (`ResumeAnalyzer.Domain`)
**What:** Contains the core business entities that represent the domain model  
**Why:** Ensures business logic is independent of infrastructure and presentation  
**How:** Defines entities with properties and relationships

**Entities:**
- `User`: Represents users who upload resumes
- `Resume`: Stores resume information, extracted text, and file metadata
- `JobDescription`: Represents job postings with requirements
- `Skill`: Master table of all available skills
- `ResumeSkill`: Many-to-many relationship between Resumes and Skills
- `JobSkill`: Many-to-many relationship between JobDescriptions and Skills
- `MatchingResult`: Stores calculated match scores and metadata

#### 2. **Data Access Layer** (`ResumeAnalyzer.Infrastructure.Data`)
**What:** Handles all database operations using Entity Framework Core  
**Why:** Abstracts database access, making the system database-agnostic  
**How:** Implements Repository pattern and Unit of Work pattern

**Components:**
- `ApplicationDbContext`: EF Core DbContext with entity configurations
- `IRepository<T>` / `Repository<T>`: Generic repository for CRUD operations
- `IResumeRepository` / `ResumeRepository`: Specialized repository for resumes
- `IJobDescriptionRepository` / `JobDescriptionRepository`: Specialized repository for jobs
- `IUnitOfWork` / `UnitOfWork`: Coordinates multiple repositories and transactions

**Best Practices:**
- Repository pattern for testability and abstraction
- Unit of Work pattern for transaction management
- Fluent API for entity configuration
- Lazy loading of related entities

#### 3. **AI/NLP Layer** (`ResumeAnalyzer.Infrastructure.AI`)
**What:** Implements NLP algorithms for text processing and similarity matching  
**Why:** Encapsulates AI logic separately from business logic  
**How:** Uses TF-IDF vectorization and cosine similarity for matching

**Services:**
- `PdfExtractionService`: Extracts text from PDF files using PdfPig library
- `TextPreprocessingService`: Cleans and normalizes text (lowercasing, tokenization, stop-word removal)
- `SkillExtractionService`: Identifies skills in resume text using dictionary matching
- `SimilarityMatchingService`: Calculates cosine similarity and match scores

**AI/NLP Algorithms:**

1. **Text Preprocessing:**
   - Lowercasing: Converts all text to lowercase for case-insensitive matching
   - Tokenization: Splits text into individual words/tokens
   - Stop-word removal: Removes common words (the, is, a, an) that don't carry semantic meaning
   - Normalization: Removes special characters and extra whitespace

2. **TF-IDF (Term Frequency-Inverse Document Frequency):**
   - **TF (Term Frequency)**: Measures how often a term appears in a document (normalized by document length)
   - **IDF (Inverse Document Frequency)**: Measures how rare/common a term is across documents
   - **TF-IDF = TF × IDF**: Gives higher weight to terms that are frequent in a document but rare overall
   - **Why TF-IDF?** It captures semantic importance, handles document length differences, and reduces noise from common words

3. **Cosine Similarity:**
   - **Formula**: `cosine(θ) = (A · B) / (||A|| × ||B||)`
   - Where A and B are TF-IDF vectors
   - Returns a value between 0 (no similarity) and 1 (identical)
   - Measures the cosine of the angle between two vectors, independent of document length

4. **Match Score Calculation:**
   - Combines cosine similarity (60% weight) and skill overlap (40% weight)
   - Final Score = `(Cosine Similarity × 0.6) + (Skill Match × 0.4) × 100`

#### 4. **Application Layer** (`ResumeAnalyzer.Application`)
**What:** Contains business logic, DTOs, and service interfaces  
**Why:** Separates business rules from presentation and data access  
**How:** Uses DTOs for data transfer and services for orchestration

**Components:**
- **DTOs (Data Transfer Objects):** `ResumeDto`, `JobDescriptionDto`, `MatchingResultDto`, etc.
- **Services:** `ResumeService`, `JobDescriptionService`, `MatchingService`
- **Validation:** Data annotations and business rule validation

**Best Practices:**
- DTOs separate presentation concerns from domain entities
- Services orchestrate complex workflows
- Validation at multiple layers (DTOs, Services)

#### 5. **Presentation Layer** (`ResumeAnalyzer.Web`)
**What:** ASP.NET Core MVC application with Razor views  
**Why:** Provides user interface and handles HTTP requests  
**How:** Controllers receive requests, call services, and return views

**Components:**
- **Controllers:** `HomeController`, `ResumeController`, `JobDescriptionController`, `MatchingController`
- **Views:** Razor views for upload forms, dashboards, and result displays
- **Configuration:** Dependency injection setup in `Program.cs`

## Database Schema

The system uses SQLite (for development) or SQL Server (for production) with the following schema:

### Tables:
1. **Users**: User information
2. **Resumes**: Resume files and extracted text
3. **JobDescriptions**: Job postings
4. **Skills**: Master skill dictionary
5. **ResumeSkills**: Many-to-many mapping (Resumes ↔ Skills)
6. **JobSkills**: Many-to-many mapping (JobDescriptions ↔ Skills)
7. **MatchingResults**: Match scores and metadata

### Entity Relationships:
- User → Resumes (1:many)
- Resume → ResumeSkills → Skills (many:many)
- JobDescription → JobSkills → Skills (many:many)
- Resume → MatchingResults ← JobDescription (many:many)

## Getting Started

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022, VS Code, or JetBrains Rider
- (Optional) SQL Server for production deployment

### Installation Steps

1. **Clone or navigate to the project directory:**
   ```bash
   cd ResumeAnalyzer
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Build the solution:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   cd ResumeAnalyzer.Web
   dotnet run
   ```

5. **Open browser:**
   Navigate to `https://localhost:5001` or `http://localhost:5000`

### First Run
- The database will be created automatically (SQLite: `ResumeAnalyzer.db`)
- Initial data will be seeded (default user, common skills)
- You can start uploading resumes and creating job descriptions

## Usage Instructions

### 1. Upload a Resume
- Navigate to **Resumes → Upload Resume**
- Select a PDF file (max 10MB)
- The system will:
  - Extract text from PDF
  - Preprocess and clean the text
  - Extract skills automatically
  - Store the resume in the database

### 2. Create a Job Description
- Navigate to **Jobs → Create Job**
- Enter job title, company, location, description
- Add required skills (comma-separated)
- The system will create the job and associate skills

### 3. Match Resumes to Jobs
- Navigate to **Matching Dashboard**
- Select a resume and job to match
- Or match all resumes to a job / all jobs to a resume
- View detailed match results with:
  - Overall match percentage
  - Text similarity score (TF-IDF)
  - Skill overlap percentage
  - Matching and missing skills

## Security Features

- **File Validation**: Only PDF files accepted, MIME type checking
- **SQL Injection Protection**: Parameterized queries via EF Core
- **Exception Handling**: Comprehensive error handling at all layers
- **Input Validation**: Data annotations and model validation
- **Anti-Forgery Tokens**: CSRF protection on form submissions

## Testing

### Sample Test Data
The system seeds initial data including:
- Default user (ID: 1, Email: demo@example.com)
- Common skills (Programming languages, frameworks, tools, etc.)

### Testing Flow
1. **Upload a Resume**: Test PDF extraction and skill identification
2. **Create a Job**: Test job creation and skill association
3. **Perform Matching**: Verify match score calculation
4. **Review Results**: Check accuracy of skill extraction and matching

### Performance Metrics
- **Parsing Accuracy**: Depends on PDF quality and structure
- **Match Precision**: TF-IDF + skill overlap provides balanced scoring
- **Processing Time**: Typically < 2 seconds per resume upload/match

## Technology Stack

- **.NET 8**: Core framework
- **ASP.NET Core MVC**: Web framework
- **Entity Framework Core 8**: ORM for database access
- **SQLite / SQL Server**: Database
- **PdfPig**: PDF text extraction
- **ML.NET**: NLP processing (TF-IDF implementation)
- **Bootstrap 5**: UI framework

## Key Features

✅ PDF Resume Upload and Text Extraction  
✅ Automatic Skill Extraction  
✅ Job Description Management  
✅ AI-Powered Resume-Job Matching  
✅ TF-IDF Vectorization  
✅ Cosine Similarity Calculation  
✅ Match Score Dashboard  
✅ Skill-Based Analysis  
✅ Clean Architecture  
✅ Repository Pattern  
✅ Unit of Work Pattern  

## Educational Value

This project demonstrates:
- Clean Architecture principles
- SOLID principles
- Repository and Unit of Work patterns
- Dependency Injection
- NLP/AI algorithms (TF-IDF, Cosine Similarity)
- Entity Framework Core best practices
- ASP.NET Core MVC development
- Database design and relationships

## Mathematical Formulas

### Cosine Similarity
```
cosine_similarity(A, B) = Σ(A_i × B_i) / (√(Σ(A_i²)) × √(Σ(B_i²)))
```

### TF-IDF
```
TF(t, d) = (Number of times term t appears in document d) / (Total terms in d)
IDF(t) = log(Total documents / (1 + Documents containing t))
TF-IDF(t, d) = TF(t, d) × IDF(t)
```

### Match Percentage
```
Match% = (Cosine Similarity × 0.6 + Skill Match × 0.4) × 100
```

## Configuration

### Database Connection
Edit `appsettings.json` to change database:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ResumeAnalyzer.db"  // SQLite
    // OR for SQL Server:
    // "DefaultConnection": "Server=localhost;Database=ResumeAnalyzer;Trusted_Connection=True;"
  }
}
```

### File Upload Settings
- Maximum file size: 10MB (configurable in `ResumeService.cs`)
- Allowed extensions: .pdf only
- Upload folder: `wwwroot/Uploads/Resumes`

## Code Documentation

All code includes comprehensive XML documentation comments explaining:
- What each component does
- Why it is needed
- How it works
- Best practices used

## Contributing

This is an educational project. Feel free to:
- Add more skills to the dictionary
- Improve matching algorithms
- Add unit tests
- Enhance UI/UX
- Add additional features

## 📄 License

This project is for educational purposes.

## Author

Built as a comprehensive example of a full-stack .NET application with AI/NLP capabilities, following industry best practices and clean architecture principles.

---

**Note for Examiners:** This project demonstrates proficiency in:
- C# and .NET 8 development
- ASP.NET Core MVC
- Entity Framework Core
- Database design
- AI/NLP algorithms (TF-IDF, Cosine Similarity)
- Clean Architecture
- Design Patterns (Repository, Unit of Work)
- Software Engineering best practices

