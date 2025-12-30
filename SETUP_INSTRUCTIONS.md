# Setup Instructions - How to Run the Project

## ⚠️ Important: Permission Issue Fix

If you encounter permission errors when running `dotnet restore` or `dotnet build`, follow these steps:

### For macOS/Linux:
1. **Fix dotnet SDK permissions** (if you see permission denied errors):
   ```bash
   sudo chown -R $(whoami) /usr/local/share/dotnet
   ```

2. **Or use a different approach** - restore packages in your IDE (Visual Studio, Rider, or VS Code)

### For Windows:
- Run Visual Studio or Command Prompt as Administrator if you encounter permission issues

---

## 🚀 Step-by-Step Setup

### Step 1: Verify .NET 8 SDK is Installed
```bash
dotnet --version
```
Should show version 8.x or higher. If not, download from: https://dotnet.microsoft.com/download/dotnet/8.0

### Step 2: Navigate to Project Directory
```bash
cd /Users/kumar/Desktop/dev/projects/resume
```

### Step 3: Restore NuGet Packages
```bash
dotnet restore
```

**If you get permission errors**, try:
- Running in your IDE (Visual Studio, Rider, VS Code)
- Or fix permissions (see above)

### Step 4: Build the Solution
```bash
dotnet build
```

This should compile all projects. Fix any errors that appear.

### Step 5: Run the Application
```bash
cd ResumeAnalyzer.Web
dotnet run
```

Or run the entire solution:
```bash
dotnet run --project ResumeAnalyzer.Web
```

### Step 6: Open Browser
Navigate to:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

The exact ports will be shown in the console output.

---

## 🔧 Common Issues and Solutions

### Issue 1: "Access to the path '/usr/local/share/dotnet/sdk/...' is denied"

**Solution:**
```bash
sudo chown -R $(whoami) /usr/local/share/dotnet
```

Or use your IDE to restore packages instead of command line.

### Issue 2: "Package restore failed"

**Solution:**
- Check your internet connection
- Try clearing NuGet cache:
  ```bash
  dotnet nuget locals all --clear
  dotnet restore
  ```

### Issue 3: "Database not found" or SQLite errors

**Solution:**
- The database is created automatically on first run
- If issues persist, delete `ResumeAnalyzer.db` file and restart
- Check that the application has write permissions in the project directory

### Issue 4: "Port already in use"

**Solution:**
- Change the port in `Properties/launchSettings.json`
- Or stop other applications using ports 5000/5001
- Or specify a different port:
  ```bash
  dotnet run --urls "http://localhost:5002"
  ```

### Issue 5: "File upload folder not found"

**Solution:**
- The `wwwroot/Uploads/Resumes` folder is created automatically
- If not, create it manually:
  ```bash
  mkdir -p ResumeAnalyzer.Web/wwwroot/Uploads/Resumes
  ```

---

## 📋 Required Dependencies

The project uses these NuGet packages (automatically restored):

### Infrastructure.Data:
- EntityFrameworkCore 8.0.0
- EntityFrameworkCore.SqlServer 8.0.0
- EntityFrameworkCore.Sqlite 8.0.0

### Infrastructure.AI:
- UglyToad.PdfPig 0.1.8
- Microsoft.ML 3.0.1
- Microsoft.ML.TextAnalytics 0.20.1

### Application:
- Microsoft.AspNetCore.App (Framework Reference)

All packages will be automatically restored when you run `dotnet restore`.

---

## ✅ Verification Checklist

Before running, ensure:

- [ ] .NET 8 SDK is installed (`dotnet --version`)
- [ ] All projects are in the solution
- [ ] `dotnet restore` completes without errors
- [ ] `dotnet build` completes successfully
- [ ] No compilation errors in IDE
- [ ] Database file will be created on first run (SQLite: `ResumeAnalyzer.db`)

---

## 🎯 First Run

1. Run the application
2. Navigate to the home page
3. The database will be created automatically
4. Initial data will be seeded:
   - Default user (ID: 1)
   - Common skills (80+ skills)
5. Start uploading resumes and creating jobs!

---

## 💡 Alternative: Using Visual Studio / Rider / VS Code

### Visual Studio:
1. Open `ResumeAnalyzer.sln`
2. Right-click solution → Restore NuGet Packages
3. Press F5 to run

### Rider:
1. Open `ResumeAnalyzer.sln`
2. Wait for indexing and package restore
3. Click Run button

### VS Code:
1. Open the project folder
2. Terminal → Run Task → restore
3. Press F5 to run (with C# extension installed)

---

## 📞 Still Having Issues?

1. Check that all `.csproj` files have correct package references
2. Verify project references are correct in solution
3. Try cleaning and rebuilding:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```
4. Check the README.md for architecture details
5. Review code comments for implementation details

Good luck! 🚀

