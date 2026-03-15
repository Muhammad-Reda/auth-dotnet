namespace backend.Endpoints;

using backend.Data;
using backend.Dto.Auth;
using backend.Dto.Users;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

public static class Auth
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        // Login
        group.MapPost("/login", async (LoginDto loginData, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.AnyAsync(u => u.Username == loginData.Username);
            if (!userExist) throw new NotFoundException("Username or Password wrong");

            var user = await dbContext.Users
                                            .Where(u => u.Username == loginData.Username)
                                            .Select(u => new AllUserDataDto(u.Id, u.Email, u.Username, u.PasswordHash, u.Role, u.IsDeleted, u.CreatedAt, u.UpdatedAt, u.DeletedAt))
                                            .AsNoTracking()
                                            .SingleAsync();
            var mathced = BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash);
            if (!mathced) throw new NotFoundException("Username or Password wrong");

            UserDetailsDto userDetail = new(
                user.Id,
                user.Email,
                user.Username,
                user.Role,
                user.IsDeleted,
                user.CreatedAt,
                user.UpdatedAt,
                user.DeletedAt
            );

            return Results.Ok(new { message = "Login Success", data = userDetail });
        });

        // Register
        group.MapPost("/register", async (RegisterDto registerData, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.AnyAsync(u => u.Email == registerData.Email || u.Username == registerData.Username);
            if (userExist) throw new ConflictException("Username or Email already exist");

            var passwordHased = BCrypt.Net.BCrypt.HashPassword(registerData.Password);
            User user = new()
            {
                Email = registerData.Email,
                PasswordHash = passwordHased,
                Role = User.UserRole.User,
                Username = registerData.Username,
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Results.Ok(new { message = "Register succes" });

        });

        // TODO:
        // Reset password
    }
}
