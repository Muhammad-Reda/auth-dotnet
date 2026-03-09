using backend.Data;
using backend.Dto.Users;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/user");

        // Get all users
        group.MapGet("/", async (ApplicationDbContext dbContext) =>
        {
            var data = await dbContext.Users
                                .Select(user => new UserDetailsDto(user.Id, user.Email, user.Username, user.Role, user.IsDeleted, user.CreatedAt, user.UpdatedAt, user.DeletedAt))
                                .AsNoTracking()
                                .ToListAsync();
            return Results.Ok(data);
        });

        // Get a user
        group.MapGet("/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.FindAsync(id);
            if (userExist is null) return Results.NotFound(new { message = "User not found" });

            UserDetailsDto userDeatils = new(
                userExist.Id,
                userExist.Email,
                userExist.Username,
                userExist.Role,
                userExist.IsDeleted,
                userExist.CreatedAt,
                userExist.UpdatedAt,
                userExist.DeletedAt
            );
            return Results.Ok(new { data = userDeatils });
        });

        // Create new user
        group.MapPost("/", async (CreateUserDto newData, ApplicationDbContext dbContent) =>
        {
            var passwordHashed = BCrypt.Net.BCrypt.HashPassword(newData.Password);

            User user = new()
            {
                Username = newData.Username,
                Email = newData.Email,
                PasswordHash = passwordHashed,
                Role = newData.Role
            };

            await dbContent.AddAsync(user);
            await dbContent.SaveChangesAsync();

            UserDetailsDto userDto = new(
                user.Id,
                user.Email,
                user.Username,
                user.Role,
                user.IsDeleted,
                user.CreatedAt,
                user.UpdatedAt,
                user.DeletedAt
            );

            return Results.Ok(new { message = "User created", data = userDto });
        });

        // Update a user
        group.MapPut("/{id}", async (Guid id, UpdateUserDto newData, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.FindAsync(id);
            if (userExist is null) return Results.NotFound(new { message = "User not found" });

            var emailExist = await dbContext.Users.AnyAsync(u => u.Email == newData.Email);
            if (emailExist) return Results.Conflict(new { message = "Email already exist" });
            var usernamelExist = await dbContext.Users.AnyAsync(u => u.Username == newData.Username);
            if (usernamelExist) return Results.Conflict(new { message = "Username already exist" });

            await dbContext.Users
                            .Where(u => u.Id == id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(u => u.Email, newData.Email)
                                .SetProperty(u => u.Username, newData.Username)
                                .SetProperty(u => u.Role, newData.Role)
                                .SetProperty(u => u.UpdatedAt, DateTime.Now));

            return Results.Ok(new { message = "User updated" });
        });

        // Delete a user
        group.MapDelete("/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.FindAsync(id);
            if (userExist is null) return Results.NotFound(new { message = "User not found" });

            await dbContext.Users
                            .Where(u => u.Id == id)
                            .ExecuteDeleteAsync();

            return Results.Ok(new { message = "User deleted" });
        });
    }
}
