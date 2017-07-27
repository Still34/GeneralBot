using Discord.Commands;

namespace GeneralBot.Results
{
    internal class CommandRuntimeResult : RuntimeResult
    {
        public CommandRuntimeResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public CommandRuntimeResult(CommandError? error, ResultType type, string reason) : base(error, reason) => Type = type;

        public ResultType Type { get; set; }

        public static CommandRuntimeResult FromError(object reason) => new CommandRuntimeResult(CommandError.Unsuccessful, ResultType.Error, reason.ToString());
        public static CommandRuntimeResult FromInfo(object reason) => new CommandRuntimeResult(null, ResultType.Info, reason.ToString());
        public static CommandRuntimeResult FromSuccess(object reason = null) => new CommandRuntimeResult(null, ResultType.Success, reason?.ToString());
    }
}