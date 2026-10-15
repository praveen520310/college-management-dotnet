using CollegeManagement.Api.Data;
using CollegeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesWriteController : ControllerBase
{
    private readonly CollegeDbContext _db;

    public CoursesWriteController(CollegeDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> AddCourse([FromBody] Course course)
    {
        if (string.IsNullOrWhiteSpace(course.Name))
            return BadRequest("Course name is required.");

        var department = await _db.Departments.FindAsync(course.DepartmentId);

        if (department == null)
            return BadRequest("Department not found.");

        var newCourse = new Course
        {
            Name = course.Name.Trim(),
            DepartmentId = course.DepartmentId
        };

        _db.Courses.Add(newCourse);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            newCourse.Id,
            newCourse.Name,
            Department = new { department.Id, department.Name }
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _db.Courses.FindAsync(id);

        if (course == null)
            return NotFound("Course not found.");

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();

        return Ok("Course deleted successfully.");
    }
}
