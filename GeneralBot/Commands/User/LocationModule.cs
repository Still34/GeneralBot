using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Models.Context;
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
        public UserContext UserSettings { get; set; }

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
        [Alias("search")]
        public async Task<RuntimeResult> LocationLookup([Remainder] string location)
        {
            var geocodeResults = (await Geocoding.GeocodeAsync(location)).ToList();
            if (!geocodeResults.Any()) return CommandRuntimeResult.FromError("No results found.");

            var embed = EmbedTemplates.FromInfo();
            foreach (var geocodeResult in geocodeResults)
            {
                if (embed.Fields.Count > 10) break;
                embed.AddField(geocodeResult.FormattedAddress, geocodeResult.Coordinates);
            }
            await ReplyAsync("", embed: embed);
            return CommandRuntimeResult.FromSuccess();
        }

        [Command("set")]
        [Alias("edit")]
        public async Task<RuntimeResult> LocationSet([Remainder] string location)
        {
            var geocodeResults = (await Geocoding.GeocodeAsync(location)).ToList();
            if (!geocodeResults.Any()) return CommandRuntimeResult.FromError("No results found.");

            var result = geocodeResults.FirstOrDefault();
            var dbEntry = UserSettings.Coordinates.SingleOrDefault(x => x.UserId == Context.User.Id) ?? UserSettings.Coordinates.Add(new Coordinate {UserId = Context.User.Id}).Entity;
            dbEntry.Longitude = result.Coordinates.Longitude;
            dbEntry.Latitude = result.Coordinates.Latitude;
            UserSettings.Update(dbEntry);
            await UserSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Your location has been set to {result.FormattedAddress}!");
        }
    }
}