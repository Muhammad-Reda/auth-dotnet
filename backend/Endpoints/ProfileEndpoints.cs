using backend.Data;
using backend.Dto.Profiles;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/profile");

        // Get user's profile
        group.MapGet("/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            // Check if user exist
            var userExist = await dbContext.Users.FindAsync(id);
            if (userExist is null) return Results.NotFound(new { message = "User not found" });

            // Check if profile exist
            var profileExist = await dbContext.Profiles.FindAsync(id);
            if (profileExist is null) return Results.NotFound(new { message = "Profile not found" });

            ProfileDetailsDto profile = new(
                Id: profileExist.Id,
                FullName: profileExist.FullName,
                Age: profileExist.Age,
                Phone: profileExist.Phone ?? "No phone number",
                Address: profileExist.Address ?? "No address",
                CreatedAt: profileExist.CreatedAt,
                UpdatedAt: profileExist.UpdatedAt,
                DeletedAt: profileExist.DeletedAt
            );

            return Results.Ok(new { data = profile });
        });

        // Create user's profile
        group.MapPost("/{userId}", async (Guid userId, CreateProfileDto newData, ApplicationDbContext dbContext) =>
        {
            // Check is user exist
            var userExist = await dbContext.Users.FindAsync(userId);
            if (userExist is null) return Results.NotFound(new { message = "User not found" });

            // Check is profile exist
            var profileExist = await dbContext.Profiles.FindAsync(userId);
            if (profileExist is not null) return Results.Conflict(new { message = "Profile already existed" });

            Profile profile = new()
            {
                Id = userExist.Id,
                FullName = newData.FullName,
                Age = newData.Age,
                Address = newData.Address,
                Phone = newData.Phone
            };

            await dbContext.AddAsync(profile);
            await dbContext.SaveChangesAsync();

            ProfileDetailsDto profileDetails = new(
                profile.Id,
                profile.FullName,
                profile.Age,
                profile.Phone ?? "No phone number",
                profile.Address ?? "No address",
                profile.CreatedAt,
                profile.UpdatedAt,
                profile.DeletedAt
            );

            return Results.Ok(new { message = "Profile created", data = profileDetails });
        });

        // Update user's profile
        group.MapPut("/{id}", async (Guid id, UpdateProfileDto newData, ApplicationDbContext dbContext) =>
        {
            // Check is user exist
            var userExist = await dbContext.Users.FindAsync(id);
            if (userExist is null) return Results.NotFound(new { message = "User not found" });

            // Check is profile exist
            var profileExist = await dbContext.Profiles.FindAsync(id);
            if (profileExist is null) return Results.NotFound(new { message = "Profile not found" });

            await dbContext.Profiles
                            .Where(p => p.Id == id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(p => p.FullName, newData.FullName)
                                .SetProperty(p => p.Age, newData.Age)
                                .SetProperty(p => p.Address, newData.Address)
                                .SetProperty(p => p.Phone, newData.Phone)
                                .SetProperty(p => p.UpdatedAt, DateTime.Now));

            return Results.Ok(new { message = "Profile updated" });
        });

        // Delete user's profile
        group.MapDelete("/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            // Check is user exist
            var userExist = await dbContext.Users.FindAsync(id);
            if (userExist is null) return Results.NotFound(new { message = "User not found" });

            // Check is profile exist
            var profileExist = await dbContext.Profiles.FindAsync(id);
            if (profileExist is null) return Results.NotFound(new { message = "Profile not found" });

            await dbContext.Profiles
                            .Where(p => p.Id == id)
                            .ExecuteDeleteAsync();

            return Results.Ok(new { message = "Profile deleted" });
        });
    }
}
