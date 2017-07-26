using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Results;
using GeneralBot.Services;
using GeneralBot.Templates;

namespace GeneralBot.Commands.User
{
    [Group("location")]
    public class LocationModule : ModuleBase<SocketCommandContext>
    {
        private IDisposable _typing;
        public GoogleGeocodingService Geocoding { get; set; }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            _typing = Context.Channel.EnterTypingState();
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            _typing.Dispose();
        }

        [Command("lookup")]
        public async Task<RuntimeResult> LocationLookup([Remainder] string location)
        {
            var geocodeResults = await Geocoding.GeocodeAsync(location);
            var embed = EmbedTemplates.FromInfo();
            foreach (var geocodeResult in geocodeResults)
            {
                if (embed.Fields.Count > 10) break;
                embed.AddField(geocodeResult.FormattedAddress, geocodeResult.Coordinates);
            }
            await ReplyAsync("", embed: embed);
            return CommandRuntimeResult.FromSuccess();
        }
    }
}