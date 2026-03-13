using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    public static void AddBackend(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["Auth:SqlServerConnection"];
        builder.Services.AddSqlServer<ApplicationDbContext>(
            connectionString
        );
    }
}
