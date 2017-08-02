using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkSky.Models;
using Discord;
using Discord.Addons.EmojiTools;
using GeneralBot.Extensions;
using Geocoding.Google;
using Humanizer;

namespace GeneralBot.Services
{
    public class WeatherService
    {
        private readonly LoggingService _loggingService;

        public WeatherService(LoggingService loggingService) => _loggingService = loggingService;

        public async Task<(List<EmbedBuilder> WeatherResults, List<EmbedBuilder> Alerts)> GetWeatherEmbedsAsync(
            DarkSkyResponse forecast, GoogleAddress geocode)
        {
            if (forecast.IsSuccessStatus)
            {
                var alertEmbeds = new List<EmbedBuilder>();
                var weatherEmbeds = new List<EmbedBuilder>();
                var response = forecast.Response;
                var location = await GetShortLocation(geocode);
                var hourlyDataPoint = response.Hourly.Data.FirstOrDefault();
                if (hourlyDataPoint != null)
                {
                    var weatherIcons = GetWeatherEmoji(hourlyDataPoint.Icon);
                    var color = CreateWeatherColor(hourlyDataPoint.Temperature);
                    var embed = new EmbedBuilder
                    {
                        Color = color,
                        Title = $"{location.Flag ?? ""}{location.Address}",
                        Description = $"Weather from {hourlyDataPoint.DateTime.Humanize()}",
                        Footer = new EmbedFooterBuilder
                        {
                            Text = "Powered by Dark Sky",
                            IconUrl = "https://darksky.net/images/darkskylogo.png"
                        },
                        ThumbnailUrl = weatherIcons.Url ?? "https://i.imgur.com/rUYrc0E.png"
                    }.WithCurrentTimestamp();
                    embed.AddInlineField($"{weatherIcons.Emoji.Name} Summary", response.Hourly.Summary);
                    embed.AddInlineField("🌡 Temperature",
                        $"{hourlyDataPoint.Temperature} °C / {hourlyDataPoint.Temperature * 1.8 + 32} °F");
                    embed.AddInlineField("☂ Precipitation", string.Format("{0:P1}", hourlyDataPoint.PrecipProbability));
                    if (hourlyDataPoint.PrecipIntensity.HasValue && hourlyDataPoint.PrecipIntensity.Value > 0)
                        embed.AddInlineField("💧 Precipitation Intensity", $"{hourlyDataPoint.PrecipIntensity} (mm/h)");
                    embed.AddInlineField("💧 Humidity", string.Format("{0:P1}", hourlyDataPoint.Humidity));
                    embed.AddInlineField("🌬 Wind Speed", $"{hourlyDataPoint.WindSpeed} (m/s)");
                    weatherEmbeds.Add(embed);
                }
                if (response.Alerts != null)
                {
                    foreach (var alert in response.Alerts)
                    {
                        var expiration = alert.ExpiresDateTime;
                        string alertContent = alert.Description.Humanize();
                        if (alertContent.Length >= 512)
                        {
                            alertContent = alertContent.Truncate(512) +
                                           "\n\nPlease click on the title for more details.";
                        }
                        var sb = new StringBuilder();
                        sb.AppendLine(Format.Bold(alert.Title));
                        sb.AppendLine();
                        sb.AppendLine(alertContent);
                        sb.AppendLine();
                        if (expiration > DateTimeOffset.UtcNow)
                            sb.AppendLine(Format.Italics($"Expires {expiration.Humanize()}"));
                        var alertEmbed = new EmbedBuilder
                        {
                            Color = Color.DarkOrange,
                            Title = $"Alert for {location.Address}, click me for more details.",
                            Url = alert.Uri.AbsoluteUri,
                            ThumbnailUrl = "https://i.imgur.com/euWbiQP.png",
                            Description = sb.ToString()
                        };
                        alertEmbeds.Add(alertEmbed);
                    }
                }
                return (weatherEmbeds, alertEmbeds);
            }
            await _loggingService.Log($"Weather returned unexpected response: {forecast.ResponseReasonPhrase}",
                LogSeverity.Error).ConfigureAwait(false);
            return (null, null);
        }

        public Task<(string Address, string Flag)> GetShortLocation(GoogleAddress geocode)
        {
            var country = geocode.Components.FirstOrDefault(
                x => x.Types.Any(y => y == GoogleAddressType.Country || y == GoogleAddressType.Establishment));
            var addressTypes = geocode.Components.ToDictionary(x => x.Types.FirstOrDefault(), y => y)
                .OrderBy(x => x.Key);
            var state = addressTypes.FirstOrDefault(x => (x.Key >= GoogleAddressType.AdministrativeAreaLevel1) &
                                                         (x.Key <= GoogleAddressType.AdministrativeAreaLevel3)).Value;
            var city = addressTypes.FirstOrDefault(x => (x.Key == GoogleAddressType.Locality) |
                                                        (x.Key == GoogleAddressType.ColloquialArea)).Value;
            var sb = new StringBuilder();
            string countryFlag = null;
            try
            {
                countryFlag = EmojiExtensions.FromText($":flag_{country.ShortName.ToLower()}:").Name;
            }
            catch
            {
                // ignored
            }
            if (city != null) sb.Append($"{city.LongName}, ");
            if (state != null) sb.Append($"{state.LongName}, ");
            sb.Append(country.ShortName);
            return Task.FromResult((sb.ToString(), countryFlag));
        }

        public static Color CreateWeatherColor(double? temperature)
        {
            if (!temperature.HasValue) return Color.Default;

            const int maxTemp = 40;
            const int minTemp = -10;
            const int range = maxTemp - minTemp;
            var maxColor = new Color(244, 67, 54);
            var minColor = new Color(227, 242, 253);
            double scale = (temperature.Value - minTemp) / range;
            int r = Convert.ToInt32(minColor.R + (maxColor.R - minColor.R) * scale).LimitToRange(0, 255);
            int g = Convert.ToInt32(minColor.G + (maxColor.G - minColor.G) * scale).LimitToRange(0, 255);
            int b = Convert.ToInt32(minColor.B + (maxColor.B - minColor.B) * scale).LimitToRange(0, 255);
            return new Color(r, g, b);
        }

        public static (Emoji Emoji, string Url) GetWeatherEmoji(Icon icon)
        {
            switch (icon)
            {
                case Icon.ClearNight:
                    return (EmojiExtensions.FromText(":full_moon_with_face:"), "https://i.imgur.com/ne9f8z5.png");

                case Icon.Rain:
                    return (EmojiExtensions.FromText(":cloud_rain:"), "https://i.imgur.com/hxWU8V1.png");

                case Icon.Snow:
                    return (EmojiExtensions.FromText(":cloud_snow:"), "https://i.imgur.com/BKyZYLL.png");

                case Icon.Sleet:
                    return (EmojiExtensions.FromText(":snowflake:"), "https://i.imgur.com/BKyZYLL.png");

                case Icon.Wind:
                    return (EmojiExtensions.FromText(":wind_blowing_face:"), "https://i.imgur.com/ObyCzM8.png");

                case Icon.Fog:
                    return (EmojiExtensions.FromText(":foggy:"), null);

                case Icon.Cloudy:
                    return (EmojiExtensions.FromText(":cloud:"), "https://i.imgur.com/W1qPad4.png");

                case Icon.PartlyCloudyDay:
                case Icon.PartlyCloudyNight:
                    return (EmojiExtensions.FromText(":white_sun_cloud:"), "https://i.imgur.com/qIVnCth.png");

                case Icon.ClearDay:
                    return (EmojiExtensions.FromText(":sunny:"), "https://i.imgur.com/GF0URKg.png");

                default:
                    return (EmojiExtensions.FromText(":sparkles:"), null);
            }
        }
    }
}