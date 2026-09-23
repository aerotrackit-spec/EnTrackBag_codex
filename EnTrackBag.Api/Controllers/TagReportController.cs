using EnTrackBag.Api.DomainComponents;
using EnTrackBag.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnTrackBag.Api.Controllers;

[ApiController]
[Route("api/tag-report")]
[Authorize(Policy = "TagReport:VIEW")]
public class TagReportController : ControllerBase
{
    private readonly ITagReportDomainComponent _domain;
    public TagReportController(ITagReportDomainComponent domain)
    {
        _domain = domain;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] TagReportSearchDto request, CancellationToken ct)
    {
        try
        {
            return Ok(await _domain.SearchAsync(request, ct));
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] string tagId, CancellationToken ct)
    {
        try
        {
            var result = await _domain.GetHistoryAsync(tagId, ct);
            return result is null ? NotFound(new
            {
                message = "No eligible history was found for this tag."
            }) : Ok(result);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("export")]
    [Authorize(Policy = "TagReport:EXPORT")]
    public async Task<IActionResult> Export([FromQuery] TagReportSearchDto request, CancellationToken ct)
    {
        try
        {
            var content = await _domain.ExportAsync(request, ct);
            return File(content, "text/csv; charset=utf-8", $"TagReport-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv");
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
