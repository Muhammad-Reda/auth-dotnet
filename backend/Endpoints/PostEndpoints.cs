using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Data;
using backend.Dto.Posts;
using backend.Exceptions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/post").RequireAuthorization();

        // Get All Posts
        group.MapGet("/", async (ApplicationDbContext dbContext) =>
        {
            var posts = await dbContext.Posts
                                        .Select(post => new PostDto(post.Id,
                                                                    post.ProfileId,
                                                                    post.Content,
                                                                    post.Date,
                                                                    post.UpVote,
                                                                    post.DownVote,
                                                                    post.IsDeleted,
                                                                    post.DeletedAt
                                                                    ))
                                        .AsNoTracking()
                                        .ToListAsync();

            return Results.Ok(new { data = posts });
        });

        // Get a post
        group.MapGet("/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            var post = await dbContext.Posts.FindAsync(id) ?? throw new NotFoundException("Post not found");
            var postDto = new PostDto(
                post.Id,
                post.ProfileId,
                post.Content,
                post.Date,
                post.UpVote,
                post.DownVote,
                post.IsDeleted,
                post.DeletedAt
            );

            return Results.Ok(new { data = postDto });
        });

        // Create a post
        group.MapPost("/", async (CreatePostDto newPost, ClaimsPrincipal claim, ApplicationDbContext dbContext) =>
        {
            var userIdClaim = claim.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Results.Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            Post post = new()
            {
                Content = newPost.Content,
                ProfileId = userId
            };

            await dbContext.AddAsync(post);
            await dbContext.SaveChangesAsync();

            return Results.Created();
        });

        // Update a post
        group.MapPut("/{id}", async (Guid id, UpdatePostDto updateData, ApplicationDbContext dbContext) =>
        {
            var post = await dbContext.Posts.FindAsync(id) ?? throw new NotFoundException("Post not found");

            await dbContext.Posts
                            .Where(p => p.Id == id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(p => p.Content, updateData.Content));

            PostDto postDto = new(
                post.Id,
                post.ProfileId,
                updateData.Content,
                post.Date,
                post.UpVote,
                post.DownVote,
                post.IsDeleted,
                post.DeletedAt
            );

            return Results.Ok(new { message = "Post updated", data = postDto });
        });

        // Delete a post
        group.MapDelete("/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            var post = await dbContext.Posts.FindAsync(id) ?? throw new NotFoundException("Post not found");

            await dbContext.Posts
                            .Where(p => p.Id == id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(p => p.IsDeleted, true)
                                .SetProperty(p => p.DeletedAt, DateTime.UtcNow));

            return Results.Ok(new { message = "Post deleted" });
        });
    }
}
