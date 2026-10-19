using DragonEnglishKids.Api.Configuration;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Privacy.Consent;
using DragonEnglishKids.Api.Privacy.Requests;
using DragonEnglishKids.Api.Retention;

namespace DragonEnglishKids.Api.Privacy;

public static class PrivacyServiceCollectionExtensions
{
    public static IServiceCollection AddPrivacy(this IServiceCollection services, PrivacyOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(new SubjectPseudonymizer(options.AuditPseudonymKey));
        services.AddSingleton<INoticeCatalog>(new NoticeCatalog(NoticeCatalog.Default));
        services.AddSingleton<ConsentEvaluator>();
        services.AddScoped<IChildAccessAuthorizer, DenyAllChildAccessAuthorizer>();
        services.AddScoped<IPrivacyStore, EfPrivacyStore>();
        services.AddScoped<AuditWriter>();
        services.AddScoped<PrivacyAccessGuard>();
        services.AddScoped<ConsentService>();
        services.AddScoped<SpeechSessionService>();
        services.AddScoped<IParentVerifier, DenyAllParentVerifier>();
        services.AddScoped<PrivacyRequestService>();
        services.AddSingleton<GuestExpiryPolicy>();
        services.AddScoped<IExpiredGuestDataStore, NullExpiredGuestDataStore>();
        services.AddScoped<RetentionRunner>();
        services.AddHostedService<RetentionWorker>();
        return services;
    }

    public static IEndpointRouteBuilder MapPrivacyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1");
        group.MapNoticeEndpoints();
        group.MapConsentEndpoints();
        group.MapSpeechSessionEndpoints();
        group.MapPrivacyRequestEndpoints();
        return app;
    }
}
