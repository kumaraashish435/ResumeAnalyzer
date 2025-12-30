# How to Run the Project - Quick Guide

## ⚡ Quick Start

### Option 1: Command Line (Recommended)

1. **Open Terminal/Command Prompt**

2. **Navigate to project directory:**
   ```bash
   cd /Users/kumar/Desktop/dev/projects/resume
   ```

3. **Restore packages** (if permission error, see below):
   ```bash
   dotnet restore
   ```

4. **Build the solution:**
   ```bash
   dotnet build
   ```

5. **Run the web application:**
   ```bash
   cd ResumeAnalyzer.Web
   dotnet run
   ```

6. **Open browser** and go to: `https://localhost:5001` or `http://localhost:5000`

---

## 🔧 If You Get Permission Errors

### For macOS (if you see: "Access to the path '/usr/local/share/dotnet/...' is denied"):

**Option A - Fix permissions:**
```bash
sudo chown -R $(whoami) /usr/local/share/dotnet
```

**Option B - Use IDE instead:**
- Open the project in Visual Studio, Rider, or VS Code
- Let the IDE restore packages automatically
- Press F5 to run

### For Windows:
- Run Command Prompt or Visual Studio as Administrator

---

## 📦 Using an IDE (Easiest Method)

### Visual Studio 2022:
1. Open `ResumeAnalyzer.sln`
2. Wait for package restore (automatic)
3. Press **F5** to run
4. Browser opens automatically

### JetBrains Rider:
1. Open `ResumeAnalyzer.sln`
2. Wait for indexing (automatic package restore)
3. Click **Run** button (green play icon)
4. Browser opens automatically

### VS Code:
1. Open the project folder
2. Install C# extension if needed
3. Press **F5** and select ".NET 5+ and .NET Core"
4. Browser opens automatically

---

## ✅ What Happens on First Run

1. **Database Created**: SQLite database file `ResumeAnalyzer.db` is created automatically
2. **Data Seeded**: 
   - Default user (ID: 1, Email: demo@example.com)
   - 80+ common skills (C#, Python, SQL, etc.)
3. **Uploads Folder**: `wwwroot/Uploads/Resumes` folder is created automatically

---

## 🎯 Test the Application

1. **Upload a Resume:**
   - Go to "Resumes" → "Upload New Resume"
   - Select a PDF file
   - Click Upload

2. **Create a Job:**
   - Go to "Jobs" → "Create New Job"
   - Fill in job details and skills
   - Click Create

3. **Match Resume to Job:**
   - Go to "Matching" dashboard
   - Select resume and job
   - Click Match
   - View match results!

---

## 🐛 Common Issues

### "Package restore failed"
- **Fix**: Check internet connection, or use IDE instead

### "Port 5000/5001 already in use"
- **Fix**: Change port in `Properties/launchSettings.json` or stop other applications

### "Database file not found"
- **Fix**: Database is created automatically on first run - just wait

### "File upload folder not found"
- **Fix**: Folder is created automatically. If not, create manually:
  ```bash
  mkdir -p ResumeAnalyzer.Web/wwwroot/Uploads/Resumes
  ```

---

## 📋 Requirements Check

Before running, ensure you have:

- ✅ .NET 8 SDK installed (`dotnet --version` should show 8.x)
- ✅ All project files present
- ✅ Solution file (`ResumeAnalyzer.sln`) exists

---

## 🚀 That's It!

Your AI Resume Analyzer should now be running! 

For detailed architecture information, see **README.md**  
For troubleshooting, see **SETUP_INSTRUCTIONS.md**

Happy coding! 🎉

