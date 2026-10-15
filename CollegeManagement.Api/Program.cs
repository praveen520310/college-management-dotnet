using CollegeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".CollegeManagement.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var connectionString =
    builder.Configuration.GetConnectionString("CollegeDb");

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<CollegeDbContext>(options =>
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)));
}
else
{
    throw new InvalidOperationException(
        "Connection string 'CollegeDb' was not found in configuration.");
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSession();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
