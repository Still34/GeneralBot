using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace GeneralBot.Typereaders
{
    public class BooleanTypeReader : TypeReader
    {
        private readonly string[] _negativeKeywords =
        {
            "no",
            "disable",
            "off"
        };

        private readonly string[] _positiveKeywords =
        {
            "yes",
            "enable",
            "on"
        };

        public static Type[] Types { get; } = {typeof(bool)};

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (bool.TryParse(input, out bool result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            if (_positiveKeywords.Any(x => input.ToLower().Contains(x)))
                return Task.FromResult(TypeReaderResult.FromSuccess(true));
            if (_negativeKeywords.Any(x => input.ToLower().Contains(x)))
                return Task.FromResult(TypeReaderResult.FromSuccess(false));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Invalid response. Try true or false."));
        }
    }
}