namespace DragonEnglishKids.Api.Privacy.Consent;

public static class NoticeEndpoints
{
    public static void MapNoticeEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet(
            "/privacy/notice",
            (string? purpose, HttpContext context, INoticeCatalog notices) =>
            {
                if (!Purposes.IsSupported(purpose))
                {
                    return PrivacyResult<NoticeDocument>
                        .Fail(ProblemTypes.InvalidRequest, StatusCodes.Status400BadRequest)
                        .ToHttpResult(context);
                }

                return PrivacyResult<NoticeDocument>
                    .Ok(new NoticeDocument(purpose!, notices.CurrentVersion, NoticeContent.For(purpose!)))
                    .ToHttpResult(context);
            });
    }
}
