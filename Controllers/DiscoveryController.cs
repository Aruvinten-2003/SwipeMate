using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Common.Models; // PagedResult<>
using SwipeMate.Application.Features.Discovery; // DiscoveryProfileDto
using SwipeMate.Application.Features.Discovery.Queries.GetDiscoveryFeed; // GetDiscoveryFeedQuery

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/discovery")]
[Authorize] // diagram: [Authorize] and completed-profile policy
[Authorize(Policy = "CompletedProfile")]
public class DiscoveryController : ControllerBase
{
    private readonly GetDiscoveryFeedQueryHandler _handler;

    // solid arrow in diagram = constructor dependency
    public DiscoveryController(GetDiscoveryFeedQueryHandler handler)
    {
        _handler = handler;
    }

    // GET /api/v1/discovery?minAge=25&maxDistance=10&cursor=...
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DiscoveryProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DiscoveryProfileDto>>> GetFeed(
        [FromQuery] GetDiscoveryFeedQuery query,
        CancellationToken ct)
    {
        // query -> handler -> cursor-paginated response
        var result = await _handler.Handle(query, ct);
        return Ok(result);
    }
}
