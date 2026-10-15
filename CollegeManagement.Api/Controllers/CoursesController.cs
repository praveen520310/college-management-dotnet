using CollegeManagement.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly CollegeDbContext _db;

    public CoursesController(CollegeDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _db.Courses
            .Include(c => c.Department)
            .Select(c => new
            {
                c.Id,
                c.Name,
                Department = c.Department == null
                    ? null
                    : new
                    {
                        c.Department.Id,
                        c.Department.Name
                    }
            })
            .ToListAsync();

        return Ok(courses);
    }
}
