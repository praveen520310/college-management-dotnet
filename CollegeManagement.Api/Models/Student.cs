namespace CollegeManagement.Api.Models;

public class Student
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int Year { get; set; }

    public int Semester { get; set; }
}
