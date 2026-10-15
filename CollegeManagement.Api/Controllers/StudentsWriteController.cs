using CollegeManagement.Api.Data;
using CollegeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsWriteController : ControllerBase
{
    private readonly CollegeDbContext _db;

    public StudentsWriteController(CollegeDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent([FromBody] Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name) ||
            string.IsNullOrWhiteSpace(student.Email))
            return BadRequest("Student name and email are required.");

        if (!await _db.Users.AnyAsync(u => u.Id == student.UserId) ||
            !await _db.Courses.AnyAsync(c => c.Id == student.CourseId) ||
            !await _db.Departments.AnyAsync(d => d.Id == student.DepartmentId))
            return BadRequest("User, course or department was not found.");

        if (await _db.Students.AnyAsync(s => s.Email == student.Email))
            return Conflict("Student email already exists.");

        var usedIds = await _db.Students
            .Select(s => s.Id)
            .ToListAsync();

        var newId = 1;
        while (usedIds.Contains(newId))
            newId++;

        student.Id = newId;
        student.User = null;
        student.Course = null;
        student.Department = null;

        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Student added successfully.", id = student.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStudent(
        int id, [FromBody] Student request)
    {
        var student = await _db.Students.FindAsync(id);

        if (student == null)
            return NotFound("Student not found.");

        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Student name and email are required.");

        if (await _db.Students.AnyAsync(
            s => s.Email == request.Email && s.Id != id))
            return Conflict("Student email already exists.");

        if (!await _db.Courses.AnyAsync(c => c.Id == request.CourseId) ||
            !await _db.Departments.AnyAsync(d => d.Id == request.DepartmentId))
            return BadRequest("Course or department was not found.");

        student.Name = request.Name;
        student.Email = request.Email;
        student.Phone = request.Phone;
        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.Address = request.Address;
        student.CourseId = request.CourseId;
        student.DepartmentId = request.DepartmentId;
        student.Year = request.Year;
        student.Semester = request.Semester;

        await _db.SaveChangesAsync();

        return Ok("Student updated successfully.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _db.Students.FindAsync(id);

        if (student == null)
            return NotFound("Student not found.");

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();

        return Ok("Student deleted successfully.");
    }
}
