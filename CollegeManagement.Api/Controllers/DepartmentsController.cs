using CollegeManagement.Api.Data;
using CollegeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly CollegeDbContext _db;

    public DepartmentsController(CollegeDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        if (!_db.Database.IsRelational())
            return StatusCode(503, "Database is not configured.");

        var departments = await _db.Departments
            .Select(d => new { d.Id, d.Name })
            .ToListAsync();

        return Ok(departments);
    }
}
