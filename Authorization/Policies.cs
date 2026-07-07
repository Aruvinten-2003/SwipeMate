using Microsoft.AspNetCore.Authorization;

namespace SwipeMate.Api.Authorization;

public static class Policies
{
    public const string Member = "Member";
    public const string CompletedProfile = "CompletedProfile";
    public const string Moderator = "Moderator";
    public const string SwipeMateAdmin = "SwipeMateAdmin";

    public static AuthorizationOptions AddSwipeMatePolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Member, policy =>
            policy.RequireAuthenticatedUser());

        options.AddPolicy(CompletedProfile, policy =>
            policy.RequireAuthenticatedUser()
                .RequireClaim("profile_completed", "true"));

        options.AddPolicy(Moderator, policy =>
            policy.RequireAuthenticatedUser()
                .RequireRole("Moderator"));

        options.AddPolicy(SwipeMateAdmin, policy =>
            policy.RequireAuthenticatedUser()
                .RequireRole("Admin"));

        return options;
    }
}
