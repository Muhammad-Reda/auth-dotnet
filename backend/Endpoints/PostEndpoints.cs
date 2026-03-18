using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Data;
using backend.Dto.Posts;
using backend.Exceptions;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints;

public static class PostEndpoints
{
    public static void MapPostEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/post").RequireAuthorization();

        // Get All Posts
        group.MapGet("/", async (HttpContext context, ApplicationDbContext dbContext) =>
        {
            var pageQuery = context.Request.Query["page"];
            var pageSizeQuery = context.Request.Query["pageSize"];

            var pageSize = string.IsNullOrEmpty(pageSizeQuery) || string.IsNullOrWhiteSpace(pageSizeQuery) ? 5 : int.Parse(pageSizeQuery!);
            var page = string.IsNullOrEmpty(pageQuery) || string.IsNullOrWhiteSpace(pageQuery) ? 1 : int.Parse(pageQuery!);

            page = page < 1 ? 1 : page;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var totalPost = await dbContext.Posts.CountAsync();
            var totalPage = totalPost % pageSize != 0 ? (totalPost / pageSize) + 1 : totalPost / pageSize;

            var posts = await dbContext.Posts
                                        .OrderByDescending(p => p.Date)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
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

            return Results.Ok(new { data = posts, totalPost, page, pageSize, totalPage });
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

        // UpVote a post
        group.MapPost("/upvote/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            var post = await dbContext.Posts.FindAsync(id) ?? throw new NotFoundException("Post not found");
            var upvoteCount = await dbContext.Posts.Where(p => p.Id == id).Select(p => p.UpVote).SingleAsync();

            await dbContext.Posts
                            .Where(p => p.Id == id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(p => p.UpVote, upvoteCount + 1));

            PostDto postDto = new(
                post.Id,
                post.ProfileId,
                post.Content,
                post.Date,
                post.UpVote += 1,
                post.DownVote,
                post.IsDeleted,
                post.DeletedAt
            );

            return Results.Ok(new { message = "Upvoted", data = postDto });
        });


        // DownVote a post
        group.MapPost("/downvote/{id}", async (Guid id, ApplicationDbContext dbContext) =>
        {
            var post = await dbContext.Posts.FindAsync(id) ?? throw new NotFoundException("Post not found");
            var downVoteCount = await dbContext.Posts.Where(p => p.Id == id).Select(p => p.DownVote).SingleAsync();

            await dbContext.Posts
                            .Where(p => p.Id == id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(p => p.DownVote, downVoteCount + 1));

            PostDto postDto = new(
                post.Id,
                post.ProfileId,
                post.Content,
                post.Date,
                post.UpVote,
                post.DownVote += 1,
                post.IsDeleted,
                post.DeletedAt
            );

            return Results.Ok(new { message = "DownVoted", data = postDto });
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
