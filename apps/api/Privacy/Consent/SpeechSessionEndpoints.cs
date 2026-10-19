namespace DragonEnglishKids.Api.Privacy.Consent;

public static class SpeechSessionEndpoints
{
    public static void MapSpeechSessionEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost(
            "/children/{childId:guid}/speech-sessions/{speechSessionId:guid}/heartbeat",
            async (
                Guid childId,
                Guid speechSessionId,
                HttpContext context,
                SpeechSessionService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.HeartbeatAsync(
                    PrivacyActor.From(context.User),
                    childId,
                    speechSessionId,
                    context.TraceIdentifier,
                    cancellationToken);
                return result.ToHttpResult(context);
            });
    }
}
