using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Common.Models; // Result
using SwipeMate.Application.Features.Safety.Commands.BlockUser;
using SwipeMate.Application.Features.Safety.Commands.ReportUser;

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/safety")]
[Authorize(Policy = "Member")] // member policy + self-action protection (enforced in handlers)
public class SafetyController : ControllerBase
{
    private readonly BlockUserCommandHandler _blockUser;
    private readonly ReportUserCommandHandler _reportUser;

    // solid arrows = constructor dependencies
    public SafetyController(
        BlockUserCommandHandler blockUser,
        ReportUserCommandHandler reportUser)
    {
        _blockUser = blockUser;
        _reportUser = reportUser;
    }

    // POST /api/v1/safety/block
    [HttpPost("block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BlockUser(
        [FromBody] BlockUserCommand command,
        CancellationToken ct)
    {
        var result = await _blockUser.Handle(command, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result);
    }

    // POST /api/v1/safety/report
    [HttpPost("report")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result>> ReportUser(
        [FromBody] ReportUserCommand command,
        CancellationToken ct)
    {
        var result = await _reportUser.Handle(command, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}