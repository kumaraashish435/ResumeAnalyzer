# Quick Start Guide

## 🚀 Running the Application

1. **Open Terminal/Command Prompt** and navigate to the project directory:
   ```bash
   cd ResumeAnalyzer
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Run the application:**
   ```bash
   cd ResumeAnalyzer.Web
   dotnet run
   ```

4. **Open your browser** and navigate to:
   - `https://localhost:5001` (HTTPS)
   - OR `http://localhost:5000` (HTTP)

## 📝 First Steps

### Step 1: Upload a Resume
1. Click **"Resumes"** in the navigation menu
2. Click **"Upload New Resume"**
3. Select a PDF file (sample resume)
4. Click **"Upload and Process Resume"**
5. Wait for processing (text extraction and skill identification)

### Step 2: Create a Job Description
1. Click **"Jobs"** in the navigation menu
2. Click **"Create New Job"**
3. Fill in:
   - Job Title (e.g., "Senior Software Engineer")
   - Company (e.g., "Tech Corp")
   - Location (e.g., "New York, NY")
   - Job Description (detailed requirements)
   - Required Skills (comma-separated, e.g., "C#, Python, SQL, Azure")
4. Click **"Create Job Description"**

### Step 3: Match Resume to Job
1. Click **"Matching"** in the navigation menu
2. Select a resume and a job from the dropdowns
3. Click **"Match"**
4. View the detailed match results:
   - Overall Match Percentage
   - Text Similarity Score (TF-IDF)
   - Skill Match Score
   - Matching Skills
   - Missing Skills

## 🎯 Sample Data

The system automatically seeds:
- **Default User**: ID=1, Email=demo@example.com
- **Common Skills**: 80+ skills including programming languages, frameworks, tools, etc.

## 🔍 Understanding Match Scores

- **80-100%**: Excellent match - candidate has most required skills
- **60-79%**: Good match - candidate has many required skills
- **40-59%**: Fair match - candidate has some required skills
- **0-39%**: Poor match - candidate lacks many required skills

The score combines:
- **60%** Text Similarity (TF-IDF cosine similarity)
- **40%** Skill Overlap (percentage of required skills found)

## 🛠️ Troubleshooting

### Database Issues
- The SQLite database (`ResumeAnalyzer.db`) is created automatically
- If you need to reset: Delete the `.db` file and restart the application

### File Upload Issues
- Ensure file is PDF format
- Maximum file size: 10MB
- Check that uploads folder exists (created automatically)

### Build Issues
- Ensure .NET 8 SDK is installed: `dotnet --version`
- Restore packages: `dotnet restore`
- Clean and rebuild: `dotnet clean && dotnet build`

## 📚 Next Steps

- Upload multiple resumes
- Create multiple job descriptions
- Compare different resumes against the same job
- Analyze which skills are most commonly matched
- Experiment with different job descriptions to see how scores vary

## 💡 Tips

1. **Detailed Job Descriptions**: More detailed job descriptions lead to better text similarity scores
2. **Specific Skills**: Use specific skill names (e.g., "ASP.NET Core" not just ".NET")
3. **Skill Dictionary**: The system recognizes skills from the master dictionary
4. **Match History**: Previously matched results are cached for faster retrieval

Happy Matching! 🎉

