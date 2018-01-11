using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace GeneralBot.Typereaders
{
    /// <summary>
    ///     Parses the input as an Uri.
    /// </summary>
    public class UriTypeReader : TypeReader
    {
        public static Type[] Types { get; } = {typeof(Uri)};

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services) =>
            Task.FromResult(Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out Uri output)
                ? TypeReaderResult.FromSuccess(output)
                : TypeReaderResult.FromError(CommandError.ParseFailed, "The input is not a valid URL."));
    }
}