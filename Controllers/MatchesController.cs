using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Common.Models; // PagedResult<>, Result
using SwipeMate.Application.Features.Matches; // MatchDto
using SwipeMate.Application.Features.Matches.Commands.Unmatch;
using SwipeMate.Application.Features.Matches.Queries.GetMatches;

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/matches")]
[Authorize(Policy = "Member")] // diagram: [Authorize] member policy
public class MatchesController : ControllerBase
{
    private readonly GetMatchesQueryHandler _getMatches;
    private readonly UnmatchCommandHandler _unmatch;

    // solid arrows = constructor dependencies
    public MatchesController(
        GetMatchesQueryHandler getMatches,
        UnmatchCommandHandler unmatch)
    {
        _getMatches = getMatches;
        _unmatch = unmatch;
    }

    // GET /api/v1/matches?cursor=...&pageSize=20
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MatchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<MatchDto>>> GetMatches(
        [FromQuery] GetMatchesQuery query,
        CancellationToken ct)
    {
        var result = await _getMatches.Handle(query, ct);
        return Ok(result);
    }

    // DELETE /api/v1/matches/{matchId}
    [HttpDelete("{matchId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unmatch(
        [FromRoute] Guid matchId,
        CancellationToken ct)
    {
        // handler expects a command, not just the Guid
        var command = new UnmatchCommand(matchId);
        var result = await _unmatch.Handle(command, ct);

        return result.IsSuccess? NoContent() : NotFound();
    }
}