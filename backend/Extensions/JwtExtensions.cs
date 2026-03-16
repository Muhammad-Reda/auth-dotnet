using System.IdentityModel.Tokens.Jwt;
using System.Text;
using backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace backend.Extensions;

public static class JwtExtensions
{
    public static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("jwt"));

        var jwtOptions = builder.Configuration.GetSection("jwt").Get<JwtOptions>();
        if (string.IsNullOrWhiteSpace(jwtOptions?.Key)) throw new InvalidOperationException("Jwt key is not configured");

        /* JWT Token */
        builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Dev
                    options.IncludeErrorDetails = true; // Prod turn off
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions!.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),

                        RoleClaimType = "role",
                        NameClaimType = JwtRegisteredClaimNames.Sub
                    };
                });
    }
}
