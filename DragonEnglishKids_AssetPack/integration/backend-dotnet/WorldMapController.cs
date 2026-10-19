// Example API contract only. Wire this service to EF Core, authentication, and SRS rules.
// Suggested NEW endpoint (not present in the original PRD):
// GET /api/v1/game/{childId}/world-map
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragonEnglishKids.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/game/{childId:guid}/world-map")]
public sealed class WorldMapController(IWorldMapService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid childId, CancellationToken ct)
    {
        // Service must validate ownership/permissions for the current user.
        var result = await service.GetWorldMapForParentAsync(User, childId, ct);
        return result is null ? NotFound() : Ok(result);
    }
}

public interface IWorldMapService
{
    Task<object?> GetWorldMapForParentAsync(
        System.Security.Claims.ClaimsPrincipal parent,
        Guid childId,
        CancellationToken cancellationToken);
}

// The JSON contract is in public/mock/world-map.json.
// Server decides locked/current/completed and validates stage starts/completions.
// Do not take unlock or star state supplied by the browser as authoritative.
