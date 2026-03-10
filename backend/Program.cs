using backend.Data;
using backend.Endpoints;
using backend.Utility;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
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

// Database Migration
app.MigrateDb();

// Exception handler
app.UseExceptionHandler();

// Map Endpoints
app.MapUserEndpoints();
app.MapProfileEndpoints();

app.Run();
