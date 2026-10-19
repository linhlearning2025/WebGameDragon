using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DragonEnglishKids.Api.Configuration;

public static class PrivacyConfiguration
{
    private static readonly string[] ValuesRequiredOutsideDevelopment =
    [
        nameof(PrivacyOptions.SpeechLeaseSeconds),
        nameof(PrivacyOptions.GuestLifetimeHours),
        nameof(PrivacyOptions.GuestDeletionDelayHours),
        nameof(PrivacyOptions.PrivacyRequestTargetDays),
        nameof(PrivacyOptions.BackupPurgeDays)
    ];

    public static PrivacyOptions Bind(IConfiguration configuration, IHostEnvironment environment)
    {
        var section = configuration.GetSection(PrivacyOptions.SectionName);

        if (string.IsNullOrWhiteSpace(section[nameof(PrivacyOptions.AuditPseudonymKey)]))
        {
            throw new InvalidOperationException(
                "Required environment variable Privacy__AuditPseudonymKey is missing.");
        }

        if (!environment.IsDevelopment())
        {
            foreach (var name in ValuesRequiredOutsideDevelopment)
            {
                if (string.IsNullOrWhiteSpace(section[name]))
                {
                    throw new InvalidOperationException(
                        $"Required environment variable Privacy__{name} is missing outside Development.");
                }
            }
        }

        var options = new PrivacyOptions();
        section.Bind(options);

        if (options.SpeechLeaseSeconds <= 0
            || options.GuestLifetimeHours <= 0
            || options.GuestDeletionDelayHours <= 0
            || options.PrivacyRequestTargetDays <= 0
            || options.BackupPurgeDays <= 0)
        {
            throw new InvalidOperationException("Privacy numeric settings must be greater than zero.");
        }

        return options;
    }
}
