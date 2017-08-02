using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkSky.Services;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands.Results;
using GeneralBot.Models.Database.UserSettings;
using GeneralBot.Services;
using Geocoding.Google;

namespace GeneralBot.Commands.User
{
    [Group("weather")]
    [RequireContext(ContextType.Guild)]
    [Summary(ModuleName)]
    [Remarks("Curious to see what the weather is like at the moment? Check out the following commands!")]
    public class WeatherModule : ModuleBase<SocketCommandContext>
    {
        private const string ModuleName = "Weather Commands";

        private readonly DarkSkyService.OptionalParameters _darkSkyParams = new DarkSkyService.OptionalParameters
        {
            MeasurementUnits = "si",
            DataBlocksToExclude = new List<ExclusionBlock>
            {
                ExclusionBlock.Minutely,
                ExclusionBlock.Currently,
                ExclusionBlock.Daily
            }
        };

        private IDisposable _typing;
        public DarkSkyService DarkSkyService { get; set; }
        public GoogleGeocoder Geocoder { get; set; }
        public UserContext UserContext { get; set; }
        public WeatherService Weather { get; set; }

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

        [Command]
        [Summary("Gets the current weather of the user's location")]
        [Priority(2)]
        public async Task<RuntimeResult> GetWeatherForLocationAsync([Summary("User")] SocketUser user = null)
        {
            var userInfo = user ?? Context.User;
            var dbEntry = UserContext.Coordinates.SingleOrDefault(x => x.UserId == userInfo.Id);
            if (dbEntry == null)
            {
                return CommandRuntimeResult.FromError(
                    "Location is not set yet! Please set the location first!");
            }
            var geocodeResults = await Geocoder.ReverseGeocodeAsync(dbEntry.Latitude, dbEntry.Longitude);
            var geocode = geocodeResults.FirstOrDefault();
            if (geocode == null)
            {
                return CommandRuntimeResult.FromError(
                    "I could not find the set location! Try setting another location.");
            }
            var forecast = await DarkSkyService.GetForecast(
                geocode.Coordinates.Latitude,
                geocode.Coordinates.Longitude,
                _darkSkyParams
            );
            var embeds = await Weather.GetWeatherEmbedsAsync(forecast, geocode);
            await ReplyAsync("", embed: embeds.WeatherResults.FirstOrDefault());
            foreach (var alert in embeds.Alerts)
                await ReplyAsync("", embed: alert);
            return CommandRuntimeResult.FromSuccess();
        }

        [Command]
        [Summary("Gets the current weather of any location (i.e. Los Angeles)")]
        [Priority(1)]
        public async Task<RuntimeResult> GetWeatherForLocationAsync([Summary("Location")] [Remainder] string location)
        {
            var geocodeResults = await Geocoder.GeocodeAsync(location);
            var geocode = geocodeResults.FirstOrDefault();
            if (geocode == null)
                return CommandRuntimeResult.FromError($"I could not find {location}! Try another location.");
            var forecast = await DarkSkyService.GetForecast(
                geocode.Coordinates.Latitude,
                geocode.Coordinates.Longitude,
                _darkSkyParams);
            var embeds = await Weather.GetWeatherEmbedsAsync(forecast, geocode);
            await ReplyAsync("", embed: embeds.WeatherResults.FirstOrDefault());
            foreach (var alert in embeds.Alerts)
                await ReplyAsync("", embed: alert);
            return CommandRuntimeResult.FromSuccess();
        }
    }
}