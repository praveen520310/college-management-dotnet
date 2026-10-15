using CollegeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Data;

public class CollegeDbContext : DbContext
{
    public CollegeDbContext(DbContextOptions<CollegeDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(u => u.PasswordHash).HasColumnName("password");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");
            entity.Property(s => s.UserId).HasColumnName("user_id");
            entity.Property(s => s.CourseId).HasColumnName("course_id");
            entity.Property(s => s.DepartmentId).HasColumnName("department_id");
            entity.Property(s => s.DateOfBirth)
                .HasColumnName("date_of_birth")
                .HasColumnType("date");

            entity.HasIndex(s => s.Email).IsUnique();

            entity.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
            entity.HasOne(s => s.Course).WithMany().HasForeignKey(s => s.CourseId);
            entity.HasOne(s => s.Department).WithMany().HasForeignKey(s => s.DepartmentId);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("courses");
            entity.Property(c => c.DepartmentId).HasColumnName("department_id");
            entity.HasOne(c => c.Department).WithMany().HasForeignKey(c => c.DepartmentId);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");
            entity.HasIndex(d => d.Name).IsUnique();
        });
    }
}
