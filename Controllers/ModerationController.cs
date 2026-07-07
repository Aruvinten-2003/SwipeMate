using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipeMate.Application.Common.Models; // PagedResult<>
using SwipeMate.Application.Features.Moderation; // ReportDto
using SwipeMate.Application.Features.Moderation.Commands.ResolveReport; // ResolveReportCommand
using SwipeMate.Application.Features.Moderation.Queries.GetReports; // GetReportsQuery

namespace SwipeMate.Api.Controllers;

[ApiController]
[Route("api/v1/moderation")]
[Authorize(Policy = "Moderator")]

public class ModerationController : ControllerBase
{
    private readonly GetReportsQueryHandler _getReports;
    private readonly ResolveReportCommandHandler _resolveReport;

    // solid arrow in diagram = constructor dependency
    public ModerationController(
        GetReportsQueryHandler getReports,
        ResolveReportCommandHandler resolveReport)
    {
        _getReports = getReports;
        _resolveReport = resolveReport;
    }

    // GET /api/v1/moderation/reports?cursor=...
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ReportDto>>> GetReports(
        [FromQuery] GetReportsQuery query,
        CancellationToken ct)
    {
        var result = await _getReports.Handle(query, ct);
        return Ok(result);
    }

    // POST /api/v1/moderation/reports/{reportId}/resolve
    [HttpPost("reports/{reportId:guid}/resolve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveReport(
        [FromRoute] Guid reportId,
        [FromBody] ResolveReportCommand command,
        CancellationToken ct)
    {
        command = command with { ReportId = reportId };

        var result = await _resolveReport.Handle(command, ct);
        return result.IsSuccess ? NoContent() : NotFound();
    }
}
