using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Extensions.Helpers;
using Microsoft.Extensions.Logging;

namespace GeneralBot.Services
{
    public class LoggingService
    {
        private readonly ILogger _clientLogger;
        private readonly ILogger _commandsLogger;

        public LoggingService(DiscordSocketClient client, CommandService commandService, ILoggerFactory loggerFactory)
        {
            var loggerFactory1 = ConfigureLogging(loggerFactory);
            _clientLogger = loggerFactory1.CreateLogger("client");
            _commandsLogger = loggerFactory1.CreateLogger("commands");

            client.Log += LogDiscordAsync;
            commandService.Log += LogCommandAsync;
        }

        private static ILoggerFactory ConfigureLogging(ILoggerFactory factory)
        {
            factory
#if DEBUG
                .AddConsole(LogLevel.Trace)
#else
                .AddConsole(LogLevel.Information)
#endif
                .AddDebug();
            factory.AddFile($"Logs/{DateTime.UtcNow:MM-dd-yy}.log");
            return factory;
        }

        private Task LogCommandAsync(LogMessage message)
        {
            var sb = new StringBuilder().AppendLine(message.ToString(prependTimestamp: false));

            // Error messages for RunMode.Async commands
            if (message.Exception is CommandException command)
            {
                long errorId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                sb.Append($"ID: {errorId}");
                var _ = command.Context.Channel.SendMessageAsync("", embed:
                    EmbedHelper
                        .FromError("Uh oh...",
                            "We ran into a problem when executing this command!\n\nDon't worry, this error has been reported.")
                        .AddField("Error ID", errorId, true)
                        .WithThumbnailUrl("https://cdn.discordapp.com/emojis/288727789241237504.png").Build());
            }

            _commandsLogger.Log(GetLogLevel(message.Severity), 0,
                message,
                null,
                (msg, ex) => sb.ToString());
            return Task.CompletedTask;
        }

        private Task LogDiscordAsync(LogMessage message)
        {
            _clientLogger.Log(GetLogLevel(message.Severity), 0,
                message,
                message.Exception,
                (msg, ex) => message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        public void Log(object message, LogSeverity severity, Exception exception = null) => _clientLogger.Log(
            GetLogLevel(severity), 0,
            message,
            exception,
            (msg, ex) => message.ToString());

        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel) Math.Abs((int) severity - 5);
    }
}