using CollegeManagement.Api.Data;
using CollegeManagement.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly CollegeDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public UsersController(CollegeDbContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Name, email and password are required.");

        var email = request.Email.Trim();

        if (await _db.Users.AnyAsync(u => u.Email == email))
            return Conflict("Email already registered!");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = "USER"
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok("Registration successful!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.Trim());

        if (user == null ||
            _hasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetString("UserRole", user.Role);

        return Ok("Login successful!");
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return Unauthorized("Please login first");

        var user = await _db.Users
            .Where(u => u.Id == userId.Value)
            .Select(u => new { u.Id, u.Email, u.Role })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            HttpContext.Session.Clear();
            return Unauthorized("Please login first");
        }

        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        if (HttpContext.Session.GetString("UserRole") != "ADMIN")
            return Forbid();

        var users = await _db.Users
            .Select(u => new { u.Id, u.Name, u.Email, u.Role })
            .ToListAsync();

        return Ok(users);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        if (HttpContext.Session.GetString("UserRole") != "ADMIN")
            return Forbid();

        var user = await _db.Users.FindAsync(id);

        if (user == null)
            return NotFound("User not found.");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return Ok("User deleted successfully.");
    }

    public sealed record RegisterRequest(
        string Name, string Email, string Password);

    public sealed record LoginRequest(
        string Email, string Password);
}
