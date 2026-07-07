using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Common.Models; // Result<>
using SwipeMate.Application.Features.Matches; // MatchDto
using SwipeMate.Application.Features.Swipes.Commands.RecordSwipe;

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/swipes")]
[Authorize] // and completed-profile policy
[Authorize(Policy = "CompletedProfile")]
public class SwipesController : ControllerBase
{
    private readonly RecordSwipeCommandHandler _recordSwipe;

    // solid arrow = constructor dependency
    public SwipesController(RecordSwipeCommandHandler recordSwipe)
    {
        _recordSwipe = recordSwipe;
    }

    // POST /api/v1/swipes
    // Body: { "targetUserId": "guid", "direction": "right" | "left" }
    [HttpPost]
    [ProducesResponseType(typeof(Result<MatchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<MatchDto?>>> RecordSwipe(
        [FromBody] RecordSwipeCommand command,
        CancellationToken ct)
    {
        // Handler returns Result<MatchDto?>
        // - null = swipe recorded, no match yet
        // - MatchDto = mutual like, return the new match
        var result = await _recordSwipe.Handle(command, ct);
        return Ok(result);
    }
}