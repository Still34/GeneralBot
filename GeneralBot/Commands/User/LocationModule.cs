using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Context;
using GeneralBot.Models.Database.UserSettings;
using Geocoding.Google;

namespace GeneralBot.Commands.User
{
    [Group("location")]
    public class LocationModule : ModuleBase<SocketCommandContext>
    {
        private IDisposable _typing;
        public GoogleGeocoder Geocoding { get; set; }
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
        [Summary("Looks up a location and returns its information.")]
        public async Task<RuntimeResult> LocationLookupAsync([Remainder] string location)
        {
            var geocodeResults = (await Geocoding.GeocodeAsync(location)).ToList();
            if (!geocodeResults.Any()) return CommandRuntimeResult.FromError("No results found.");

            var embed = EmbedHelper.FromInfo();
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
        [Summary("Set your current location for time and weather commands.")]
        public async Task<RuntimeResult> LocationSetAsync([Remainder] string location)
        {
            var geocodeResults = (await Geocoding.GeocodeAsync(location)).ToList();
            if (!geocodeResults.Any()) return CommandRuntimeResult.FromError("No results found.");

            var result = geocodeResults.FirstOrDefault();
            var dbEntry = UserSettings.Coordinates.SingleOrDefault(x => x.UserId == Context.User.Id) ??
                          UserSettings.Coordinates.Add(new Coordinate {UserId = Context.User.Id}).Entity;

            dbEntry.Longitude = result.Coordinates.Longitude;
            dbEntry.Latitude = result.Coordinates.Latitude;

            await UserSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess($"Your location has been set to {result.FormattedAddress}!");
        }

        [Command("remove")]
        [Alias("delete", "wipe")]
        [Summary("Removes *all* of your location data.")]
        public async Task<RuntimeResult> LocationRemoveAsync()
        {
            var dbEntry = UserSettings.Coordinates.Where(x => x.UserId == Context.User.Id);
            if (dbEntry == null)
                return CommandRuntimeResult.FromError("You do not have a location set up yet!");
            UserSettings.RemoveRange(dbEntry);
            await UserSettings.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess("You have successfully removed your location!");
        }
    }
}