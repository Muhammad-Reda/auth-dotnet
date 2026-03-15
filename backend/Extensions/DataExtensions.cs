using Microsoft.EntityFrameworkCore;

namespace backend.Extensions;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        try
        {
            var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
        catch
        {
            // throw new UnknownException("Unkown error occured");
        }
    }

    public static void AddBackend(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration["Auth:SqlServerConnection"];
        builder.Services.AddSqlServer<ApplicationDbContext>(
            connectionString
        );
    }
}
