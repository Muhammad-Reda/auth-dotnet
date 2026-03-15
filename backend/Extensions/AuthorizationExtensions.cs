namespace backend.Extensions;

public static class AuthorizationExtensions
{
    public static void AddAuthorizationPolicy(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Reports.read", policy => policy.RequireClaim("scope", "reports.read"));
    }
}
