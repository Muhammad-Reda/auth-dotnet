using backend.Data;
using backend.Endpoints;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.AddBackend();
var app = builder.Build();


app.MapGet("/hello", () => "Hello World!");

// Database Migration
app.MigrateDb();

// Map Endpoints
app.MapUserEndpoints();
app.MapProfileEndpoints();

app.Run();
