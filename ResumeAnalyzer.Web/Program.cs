using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Application.Services;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Infrastructure.AI.Services;
using ResumeAnalyzer.Infrastructure.Data;
using ResumeAnalyzer.Infrastructure.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddControllersWithViews();

// Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=ResumeAnalyzer.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Register Repository Pattern Services
// UnitOfWork handles all repository instances internally
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register AI/NLP Services
builder.Services.AddScoped<IPdfExtractionService, PdfExtractionService>();
builder.Services.AddScoped<ITextPreprocessingService, TextPreprocessingService>();
builder.Services.AddScoped<ISkillExtractionService, SkillExtractionService>();
builder.Services.AddScoped<ISimilarityMatchingService, SimilarityMatchingService>();

// Register Application Services
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IJobDescriptionService, JobDescriptionService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Create database if it doesn't exist
        context.Database.EnsureCreated();
        
        // Seed initial data (skills, sample user)
        await SeedDataAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


/// Seed initial data into the database

static async Task SeedDataAsync(ApplicationDbContext context)
{
    // Check if data already exists
    if (context.Users.Any() || context.Skills.Any())
        return;

    // Create a default user
    var defaultUser = new User
    {
        Name = "Demo User",
        Email = "demo@example.com",
        CreatedAt = DateTime.UtcNow
    };
    context.Users.Add(defaultUser);
    await context.SaveChangesAsync();

    // Seed common skills
    var commonSkills = new[]
    {
        // Programming Languages
        "C#", "C++", "Java", "Python", "JavaScript", "TypeScript", "Ruby", "PHP", "Go", "Rust",
        "Swift", "Kotlin", "Scala", "R", "MATLAB",
        
        // Web Technologies
        "HTML", "CSS", "React", "Angular", "Vue.js", "Node.js", "ASP.NET", "ASP.NET Core",
        "Blazor", "jQuery", "Bootstrap", "Tailwind CSS",
        
        // Backend & Databases
        "SQL", "SQL Server", "MySQL", "PostgreSQL", "MongoDB", "Redis", "Oracle", "SQLite",
        "Entity Framework", "Entity Framework Core", "Dapper",
        
        // Cloud & DevOps
        "Azure", "AWS", "Google Cloud", "Docker", "Kubernetes", "CI/CD", "DevOps",
        "Git", "GitHub", "GitLab", "Jenkins", "Azure DevOps",
        
        // Frameworks & Libraries
        ".NET", ".NET Core", ".NET Framework", "Spring", "Django", "Flask", "Express.js",
        "Hibernate", "NHibernate",
        
        // Tools & Technologies
        "REST API", "GraphQL", "Microservices", "API Development", "JSON", "XML",
        "Machine Learning", "Deep Learning", "Artificial Intelligence", "Data Science",
        "Power BI", "Tableau", "Excel", "Analytics",
        
        // Soft Skills (commonly mentioned)
        "Project Management", "Agile", "Scrum", "Team Leadership", "Communication",
        "Problem Solving", "Analytical Thinking"
    };

    foreach (var skillName in commonSkills)
    {
        var category = DetermineSkillCategory(skillName);
        context.Skills.Add(new Skill
        {
            Name = skillName,
            Category = category,
            CreatedAt = DateTime.UtcNow
        });
    }

    await context.SaveChangesAsync();
}


/// Determine skill category based on skill name
static string DetermineSkillCategory(string skillName)
{
    var lowerSkill = skillName.ToLowerInvariant();
    
    if (lowerSkill.Contains("cloud") || lowerSkill.Contains("azure") || lowerSkill.Contains("aws") || 
        lowerSkill.Contains("docker") || lowerSkill.Contains("kubernetes") || lowerSkill.Contains("devops"))
        return "Cloud/DevOps";
    
    if (lowerSkill.Contains("sql") || lowerSkill.Contains("database") || lowerSkill.Contains("mongodb") || 
        lowerSkill.Contains("redis") || lowerSkill.Contains("oracle"))
        return "Database";
    
    if (lowerSkill.Contains("react") || lowerSkill.Contains("angular") || lowerSkill.Contains("vue") || 
        lowerSkill.Contains("html") || lowerSkill.Contains("css") || lowerSkill.Contains("javascript"))
        return "Frontend";
    
    if (lowerSkill.Contains("api") || lowerSkill.Contains("rest") || lowerSkill.Contains("graphql") || 
        lowerSkill.Contains("microservice"))
        return "Backend";
    
    if (lowerSkill.Contains("machine learning") || lowerSkill.Contains("ai") || 
        lowerSkill.Contains("data science") || lowerSkill.Contains("analytics"))
        return "Data Science/AI";
    
    if (lowerSkill.Contains("management") || lowerSkill.Contains("agile") || lowerSkill.Contains("scrum") || 
        lowerSkill.Contains("leadership") || lowerSkill.Contains("communication"))
        return "Soft Skills";
    
    return "Technical";
}
