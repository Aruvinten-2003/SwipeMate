using System.Security.Claims;
using SwipeMate.Application.Common.Interfaces;

namespace SwipeMate.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId =>
        Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public string? DisplayName => User?.Identity?.Name;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public IReadOnlyCollection<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray()
        ?? Array.Empty<string>();

    public bool IsInRole(string role)
    {
        return User?.IsInRole(role) == true;
    }
}
