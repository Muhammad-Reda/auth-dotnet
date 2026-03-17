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

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Exception handler
app.UseExceptionHandler();

// Database Migration
app.MigrateDb();

/* Map Endpoints */
// Auth
app.MapAuthEndpoints();

// User
app.MapUserEndpoints();

// Profile
app.MapProfileEndpoints();

// post
app.MapPostEndpoints();

app.Run();
