using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Templates;
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

            client.Log += LogDiscord;
            commandService.Log += LogCommand;
        }

        private static ILoggerFactory ConfigureLogging(ILoggerFactory factory)
        {
            factory.AddConsole();
            factory.AddFile($"Logs/{DateTime.UtcNow:MM-dd-yy}.log");
            return factory;
        }

        private Task LogCommand(LogMessage message)
        {
            var sb = new StringBuilder().AppendLine(message.ToString(prependTimestamp: false));

            // Error messages for RunMode.Async commands
            if (message.Exception is CommandException command)
            {
                long errorId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                sb.Append($"ID: {errorId}");
                var _ = command.Context.Channel.SendMessageAsync("", embed:
                    EmbedTemplates
                        .FromError(title: "Uh oh...", description: "We ran into a problem when executing this command!\n\nDon't worry, this error has been reported.")
                        .AddInlineField("Error ID", errorId)
                        .WithThumbnailUrl("https://cdn.discordapp.com/emojis/288727789241237504.png"));
            }

            _commandsLogger.Log(GetLogLevel(message.Severity), 0,
                message,
                null,
                (msg, ex) => sb.ToString());
            return Task.CompletedTask;
        }

        private Task LogDiscord(LogMessage message)
        {
            _clientLogger.Log(GetLogLevel(message.Severity), 0,
                message,
                message.Exception,
                (msg, ex) => message.ToString(prependTimestamp: false));
            return Task.CompletedTask;
        }

        public Task Log(object message, LogSeverity severity, Exception exception = null)
        {
            _clientLogger.Log(GetLogLevel(severity), 0,
                message,
                exception,
                (msg, ex) => message.ToString());
            return Task.CompletedTask;
        }

        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel) Math.Abs((int) severity - 5);
    }
}