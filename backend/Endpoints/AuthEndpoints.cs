using backend.Dto.Auth;
using backend.Dto.Users;
using backend.Exceptions;
using backend.Extensions;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoits(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        // Login
        group.MapPost("/login", async (HttpContext context, LoginDto req, IJwtService jwt, ApplicationDbContext dbContext) =>
        {
            var userExist = await dbContext.Users.AnyAsync(u => u.Username == req.Username);
            if (!userExist) throw new NotFoundException("User is not registered");

            var user = await dbContext.Users
                                        .Where(u => u.Username == req.Username)
                                        .Select(user => new UserDto(user.Id, user.Email, user.Username, user.PasswordHash, user.Role, user.IsDeleted, user.CreatedAt, user.UpdatedAt, user.DeletedAt))
                                        .AsNoTracking()
                                        .SingleAsync();

            var passMatch = BCrypt.Net.BCrypt.Verify(req.Password, user.Password);
            if (!passMatch) return Results.Unauthorized();

            var accessToken = jwt.GenerateToken(user.Id, user.Email, user.Role.ToString());
            var refreshToken = jwt.GenerateRefreshToken();
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            RefreshToken newRefreshToken = new()
            {
                Token = refreshToken,
                ExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id
            };

            await dbContext.RefreshTokens.AddAsync(newRefreshToken);
            await dbContext.SaveChangesAsync();

            context.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Results.Ok(new
            {
                AccessToken = accessToken,
                TokenType = "Bearer",
            });
        });

        // Refresh token
        group.MapPost("/refresh", async (HttpContext context, IJwtService jwt, ApplicationDbContext dbContext) =>
        {
            var refreshTokenCookie = context.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshTokenCookie)) return Results.Unauthorized();

            var refreshTokenExist = await dbContext.RefreshTokens.AnyAsync(r => r.Token == refreshTokenCookie);
            if (!refreshTokenExist) return Results.Unauthorized();


            var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshTokenCookie);
            if (token is null) return Results.Unauthorized();
            if (token.IsRevoked) return Results.Unauthorized();
            if (token.ExpiresAt < DateTime.UtcNow) return Results.Unauthorized();

            var userId = await dbContext.RefreshTokens
                                        .Where(r => r.Token == refreshTokenCookie)
                                        .Select(user => user.UserId)
                                        .SingleAsync();
            var user = await dbContext.Users.FindAsync(userId) ?? throw new Exception();

            var newAccessToken = jwt.GenerateToken(user!.Id, user.Email, user.Role.ToString());

            await dbContext.RefreshTokens
                            .Where(r => r.Token == refreshTokenCookie)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(r => r.IsRevoked, true));

            var newRefreshToken = jwt.GenerateRefreshToken();
            RefreshToken refreshTokenModel = new()
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                UserId = userId
            };
            await dbContext.RefreshTokens.AddAsync(refreshTokenModel);
            await dbContext.SaveChangesAsync();

            return Results.Ok(new
            {
                AccessToken = newAccessToken,
            });

        });

        // Logout
        group.MapPut("/logout", async (HttpContext context, ApplicationDbContext dbContext) =>
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Results.Unauthorized();
            if (string.IsNullOrWhiteSpace(refreshToken)) return Results.Unauthorized();

            var refreshTokenExistDb = await dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (refreshTokenExistDb is null) return Results.Unauthorized();
            if (refreshTokenExistDb.IsRevoked) return Results.Unauthorized();

            context.Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            await dbContext.RefreshTokens
                            .Where(r => r.Token == refreshToken)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(r => r.IsRevoked, true));

            return Results.Ok();
        });
    }
}
