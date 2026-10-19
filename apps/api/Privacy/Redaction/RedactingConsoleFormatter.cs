using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace DragonEnglishKids.Api.Privacy.Redaction;

// Central console log output: sanitizes the message and writes only the exception type, never its text.
public sealed class RedactingConsoleFormatter() : ConsoleFormatter(FormatterName)
{
    public const string FormatterName = "redacting";

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null && logEntry.Exception is null)
        {
            return;
        }

        var line = new StringBuilder()
            .Append(logEntry.LogLevel)
            .Append(": ")
            .Append(logEntry.Category)
            .Append(": ")
            .Append(RedactionPolicy.Sanitize(message));

        if (logEntry.Exception is not null)
        {
            line.Append(" [").Append(logEntry.Exception.GetType().Name).Append(']');
        }

        textWriter.WriteLine(line.ToString());
    }
}
