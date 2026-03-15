using backend.Extensions;
using backend.Endpoints;
using backend.Exceptions;
using System.Text.Json.Serialization;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();

// JWT Services
builder.Services.AddScoped<IJwtService, JwtService>();

/* JWT */
builder.AddJwtAuthentication();
builder.AddAuthorizationPolicy();

// Auth
builder.Services.AddAuthorization();

// Authorization policy
builder.AddAuthorizationPolicy();

// Enum for Role
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Global exceptions handler
builder.Services.AddExceptionHandler<GlobalExceptionsHandler>();
builder.Services.AddProblemDetails();


builder.AddBackend();
var app = builder.Build();

app.MapGet("/hello", () => "Hello World!");

// Tambahkan endpoint test ini di Program.cs
app.MapGet("/test-auth", (HttpContext ctx) =>
{
    return Results.Ok(new
    {
        isAuthenticated = ctx.User.Identity?.IsAuthenticated,
        claims = ctx.User.Claims.Select(c => new { c.Type, c.Value })
    });
}).RequireAuthorization();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Exception handler
app.UseExceptionHandler();

// Database Migration
app.MigrateDb();

/* Map Endpoints */
// Auth
app.MapAuthEndpoits();

// User
app.MapUserEndpoints();

// Profile
app.MapProfileEndpoints();
app.MapAuthEndpoints();

app.Run();
