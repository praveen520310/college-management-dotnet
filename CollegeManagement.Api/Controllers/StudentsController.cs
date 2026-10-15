using CollegeManagement.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly CollegeDbContext _db;

    public StudentsController(CollegeDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _db.Students
            .Include(s => s.User)
            .Include(s => s.Course)
            .Include(s => s.Department)
	    .OrderBy(s => s.Id)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Email,
                s.Phone,
                s.DateOfBirth,
                s.Gender,
                s.Address,
                s.Year,
                s.Semester,
                User = s.User == null ? null : new { s.User.Id },
                Course = s.Course == null ? null : new { s.Course.Id, s.Course.Name },
                Department = s.Department == null
                    ? null
                    : new { s.Department.Id, s.Department.Name }
            })
            .ToListAsync();

        return Ok(students);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStudent(int id)
    {
        var student = await _db.Students
            .Include(s => s.User)
            .Include(s => s.Course)
            .Include(s => s.Department)
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Email,
                s.Phone,
                s.DateOfBirth,
                s.Gender,
                s.Address,
                s.Year,
                s.Semester,
                User = s.User == null ? null : new { s.User.Id },
                Course = s.Course == null ? null : new { s.Course.Id, s.Course.Name },
                Department = s.Department == null
                    ? null
                    : new { s.Department.Id, s.Department.Name }
            })
            .FirstOrDefaultAsync();

        return student == null ? NotFound() : Ok(student);
    }
}
