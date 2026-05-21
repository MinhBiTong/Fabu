using Application.DTOs.Requests.Search;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class SearchController : ControllerBase
{
    private readonly IGlobalSearchService _searchService;

    public SearchController(IGlobalSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] SearchRequest request,
        CancellationToken cancellationToken)
        => ToActionResult(await _searchService.SearchAsync(request, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> SearchPost(
        [FromBody] SearchRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(ApiResponse<object>.Fail(400, "Request body khong hop le."));
        }

        return ToActionResult(await _searchService.SearchAsync(request, cancellationToken));
    }

    [HttpPost("reindex")]
    public async Task<IActionResult> Reindex(CancellationToken cancellationToken)
        => ToActionResult(await _searchService.ReindexAllAsync(cancellationToken));

    [HttpGet("health")]
    public async Task<IActionResult> Health(CancellationToken cancellationToken)
        => ToActionResult(await _searchService.HealthAsync(cancellationToken));

    private IActionResult ToActionResult<T>(ApiResponse<T> response)
    {
        return response.Code switch
        {
            200 => Ok(response),
            400 => BadRequest(response),
            404 => NotFound(response),
            503 => StatusCode(StatusCodes.Status503ServiceUnavailable, response),
            _ => StatusCode(response.Code, response)
        };
    }
}
