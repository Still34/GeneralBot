using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.Commands;

namespace GeneralBot.Typereaders
{
    public class StringTimeSpanTypeReader : TypeReader
    {
        private readonly Regex _regex = new Regex(@"(\d+)\s{0,1}([a-zA-Z]*)", RegexOptions.Compiled);

        private Task<TimeSpan> DehumanizeTimespan(string input)
        {
            var regexMatches = _regex.Matches(input);
            var timeSpan = new TimeSpan();
            foreach (Match match in regexMatches)
            {
                if (match.Groups.Count < 3) continue;
                string digit = match.Groups[1].Value;
                string unit = match.Groups[2].Value;
                if (!int.TryParse(digit, out int digitResult)) continue;
                switch (unit)
                {
                    case var s when s.IndexOf("d", StringComparison.OrdinalIgnoreCase) >= 0:
                        timeSpan = timeSpan.Add(new TimeSpan(digitResult, 0, 0, 0));
                        break;

                    case var s when s.IndexOf("h", StringComparison.OrdinalIgnoreCase) >= 0:
                        timeSpan = timeSpan.Add(new TimeSpan(digitResult, 0, 0));
                        break;

                    case var s when s.IndexOf("m", StringComparison.OrdinalIgnoreCase) >= 0:
                        timeSpan = timeSpan.Add(new TimeSpan(0, digitResult, 0));
                        break;

                    case var s when s.IndexOf("s", StringComparison.OrdinalIgnoreCase) >= 0:
                        timeSpan = timeSpan.Add(new TimeSpan(0, 0, digitResult));
                        break;
                }
            }
            return Task.FromResult(timeSpan);
        }

        public override async Task<TypeReaderResult> Read(ICommandContext context, string input,
            IServiceProvider services)
        {
            var dateTimeParsed = await DehumanizeTimespan(input);
            return dateTimeParsed == TimeSpan.Zero
                ? TypeReaderResult.FromError(CommandError.ParseFailed,
                    @"Invalid time format. Format example: `4d3h2m1s` (4 days 3 hours 2 minutes 1 second).")
                : TypeReaderResult.FromSuccess(dateTimeParsed);
        }
    }
}