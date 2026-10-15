using CollegeManagement.Api.Data;
using CollegeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsWriteController : ControllerBase
{
    private readonly CollegeDbContext _db;

    public DepartmentsWriteController(CollegeDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> AddDepartment([FromBody] Department department)
    {
        if (string.IsNullOrWhiteSpace(department.Name))
            return BadRequest("Department name is required.");

        if (await _db.Departments.AnyAsync(d => d.Name == department.Name))
            return Conflict("Department already exists.");

        var newDepartment = new Department { Name = department.Name.Trim() };

        _db.Departments.Add(newDepartment);
        await _db.SaveChangesAsync();

        return Ok(new { newDepartment.Id, newDepartment.Name });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _db.Departments.FindAsync(id);

        if (department == null)
            return NotFound("Department not found.");

        _db.Departments.Remove(department);
        await _db.SaveChangesAsync();

        return Ok("Department deleted successfully.");
    }
}
