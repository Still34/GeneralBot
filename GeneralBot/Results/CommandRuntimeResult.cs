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

        public static CommandRuntimeResult FromError(string reason) => new CommandRuntimeResult(CommandError.Unsuccessful, ResultType.Error, reason);
        public static CommandRuntimeResult FromInfo(string reason) => new CommandRuntimeResult(null, ResultType.Info, reason);
        public static CommandRuntimeResult FromSuccess(string reason = null) => new CommandRuntimeResult(null, ResultType.Success, reason);
    }
}