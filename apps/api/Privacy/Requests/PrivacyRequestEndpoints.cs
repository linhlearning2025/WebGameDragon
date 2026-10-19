namespace DragonEnglishKids.Api.Privacy.Requests;

public static class PrivacyRequestEndpoints
{
    public static void MapPrivacyRequestEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost(
            "/children/{childId:guid}/privacy-requests",
            async (
                Guid childId,
                PrivacyRequestCreateDto body,
                HttpContext context,
                PrivacyRequestService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.CreateAsync(
                    PrivacyActor.From(context.User),
                    childId,
                    body.Type,
                    context.Request.Headers["Idempotency-Key"].ToString(),
                    context.TraceIdentifier,
                    cancellationToken);
                return result.ToHttpResult(context, StatusCodes.Status202Accepted);
            });

        group.MapGet(
            "/privacy-requests/{requestId:guid}",
            async (
                Guid requestId,
                HttpContext context,
                PrivacyRequestService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.GetAsync(
                    PrivacyActor.From(context.User),
                    requestId,
                    context.TraceIdentifier,
                    cancellationToken);
                return result.ToHttpResult(context);
            });
    }
}
