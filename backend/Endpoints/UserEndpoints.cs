using backend.Data;
using backend.Dto.Users;
using backend.Exceptions;
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
                                .OrderByDescending(user => user.CreatedAt)
                                .Select(user => new UserDetailsDto(user.Id, user.Email, user.Username, user.Role, user.IsDeleted, user.CreatedAt, user.UpdatedAt, user.DeletedAt))
                                .AsNoTracking()
                                .ToListAsync();
            return Results.Ok(data);
        });

        // Get a user
        group.MapGet("/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.FindAsync(id) ?? throw new NotFoundException("User not found");

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
        group.MapPost("/", async (CreateUserDto newData, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.AnyAsync(u => u.Email == newData.Email || u.Username == newData.Username);
            if (userExist) throw new ConflictException("User already exist");

            var passwordHashed = BCrypt.Net.BCrypt.HashPassword(newData.Password);

            User user = new()
            {
                Username = newData.Username,
                Email = newData.Email,
                PasswordHash = passwordHashed,
                Role = newData.Role ?? throw new BadHttpRequestException("Role field is required")
            };

            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();

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
            var userExist = await dbContext.Users.FindAsync(id) ?? throw new NotFoundException("User not found");

            var isExist = await dbContext.Users.AnyAsync(u => u.Email == newData.Email || u.Username == newData.Username);
            if (isExist) throw new ConflictException("Email or Username already taken");

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
            var userExist = await dbContext.Users.FindAsync(id) ?? throw new NotFoundException("User not found");

            await dbContext.Users
                            .Where(u => u.Id == id)
                            .ExecuteDeleteAsync();

            return Results.Ok(new { message = "User deleted" });
        });
    }
}
