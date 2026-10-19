namespace DragonEnglishKids.Api.Privacy.Consent;

public static class ConsentEndpoints
{
    public static void MapConsentEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet(
            "/children/{childId:guid}/consents",
            async (Guid childId, HttpContext context, ConsentService service, CancellationToken cancellationToken) =>
            {
                var result = await service.GetHistoryAsync(
                    PrivacyActor.From(context.User),
                    childId,
                    context.TraceIdentifier,
                    cancellationToken);
                return result.ToHttpResult(context);
            });

        group.MapPut(
            "/children/{childId:guid}/consents/{purpose}",
            async (
                Guid childId,
                string purpose,
                ConsentDecisionRequest body,
                HttpContext context,
                ConsentService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.RecordDecisionAsync(
                    PrivacyActor.From(context.User),
                    childId,
                    purpose,
                    body,
                    context.Request.Headers["Idempotency-Key"].ToString(),
                    context.TraceIdentifier,
                    cancellationToken);
                return result.ToHttpResult(context);
            });

        group.MapPost(
            "/children/{childId:guid}/speech-sessions",
            async (Guid childId, HttpContext context, SpeechSessionService service, CancellationToken cancellationToken) =>
            {
                var result = await service.StartAsync(
                    PrivacyActor.From(context.User),
                    childId,
                    context.TraceIdentifier,
                    cancellationToken);
                return result.ToHttpResult(context);
            });
    }
}
