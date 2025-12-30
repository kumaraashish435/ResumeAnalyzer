using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Data;


/// Entity Framework Core DbContext
/// Database context for all entity operations and database access
/// Required for Entity Framework Core to manage database operations, relationships, and migrations
/// Inherits from DbContext and defines DbSets for each entity with fluent API configurations

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets represent database tables
    public DbSet<User> Users { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<JobDescription> JobDescriptions { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<ResumeSkill> ResumeSkills { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }
    public DbSet<MatchingResult> MatchingResults { get; set; }

    
    /// Configure entity relationships and database constraints using Fluent API
    /// This method is called when the model is being created
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique(); // Email must be unique
        });

        // Configure Resume entity
        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ExtractedText).HasColumnType("TEXT"); // TEXT type for large content
            
            // Relationship: One User can have many Resumes
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Resumes)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // If user is deleted, delete their resumes
        });

        // Configure JobDescription entity
        modelBuilder.Entity<JobDescription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnType("TEXT");
            entity.Property(e => e.Company).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
        });

        // Configure Skill entity (master table)
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique(); // Skill name must be unique
        });

        // Configure ResumeSkill (many-to-many junction table)
        modelBuilder.Entity<ResumeSkill>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Composite unique index: A resume cannot have the same skill twice
            entity.HasIndex(e => new { e.ResumeId, e.SkillId }).IsUnique();
            
            // Relationship: One Resume can have many ResumeSkills
            entity.HasOne(e => e.Resume)
                  .WithMany(r => r.ResumeSkills)
                  .HasForeignKey(e => e.ResumeId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Relationship: One Skill can appear in many ResumeSkills
            entity.HasOne(e => e.Skill)
                  .WithMany(s => s.ResumeSkills)
                  .HasForeignKey(e => e.SkillId)
                  .OnDelete(DeleteBehavior.Restrict); // Don't delete skill if it's used
        });

        // Configure JobSkill (many-to-many junction table)
        modelBuilder.Entity<JobSkill>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Composite unique index: A job cannot have the same skill twice
            entity.HasIndex(e => new { e.JobDescriptionId, e.SkillId }).IsUnique();
            
            // Relationship: One JobDescription can have many JobSkills
            entity.HasOne(e => e.JobDescription)
                  .WithMany(j => j.JobSkills)
                  .HasForeignKey(e => e.JobDescriptionId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Relationship: One Skill can be required by many JobSkills
            entity.HasOne(e => e.Skill)
                  .WithMany(s => s.JobSkills)
                  .HasForeignKey(e => e.SkillId)
                  .OnDelete(DeleteBehavior.Restrict); // Don't delete skill if it's used
        });

        // Configure MatchingResult entity
        modelBuilder.Entity<MatchingResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Composite unique index: A resume-job pair should have only one matching result
            entity.HasIndex(e => new { e.ResumeId, e.JobDescriptionId }).IsUnique();
            
            // Relationship: One Resume can have many MatchingResults
            entity.HasOne(e => e.Resume)
                  .WithMany(r => r.MatchingResults)
                  .HasForeignKey(e => e.ResumeId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Relationship: One JobDescription can have many MatchingResults
            entity.HasOne(e => e.JobDescription)
                  .WithMany(j => j.MatchingResults)
                  .HasForeignKey(e => e.JobDescriptionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

