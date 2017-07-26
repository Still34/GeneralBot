using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace GeneralBot.Services
{
    public class LogService
    {
        private readonly ILogger _clientLogger;
        private readonly ILogger _commandsLogger;

        public LogService(DiscordSocketClient client, CommandService commandService, ILoggerFactory loggerFactory)
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
            return factory;
        }

        private Task LogCommand(LogMessage message)
        {
            // Error messages for async commands
            if (message.Exception is CommandException command)
            {
                var _ = command.Context.Channel.SendMessageAsync($"Error Occured: {command.Message}");
            }

            _commandsLogger.Log(GetLogLevel(message.Severity), 0,
                message,
                message.Exception,
                (msg, ex) => message.ToString(prependTimestamp: false));
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