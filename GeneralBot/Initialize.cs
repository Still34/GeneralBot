using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DarkSky.Services;
using DirectoryMaid.Services;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.CoreSettings;
using GeneralBot.Models.Database.UserSettings;
using GeneralBot.Services;
using Geocoding.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GeneralBot
{
    public class Initialize
    {
        public static Task<IServiceProvider> StartAsync(DiscordSocketClient client)
        {
            var result = new Initialize();
            return result.InitAsync(client);
        }

        private async Task<IServiceProvider> InitAsync(DiscordSocketClient client)
        {
            var collection = new ServiceCollection();
            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
            var config = await ConfigureSettingsAsync();
            collection
                // Misc Services / Configs
                .AddSingleton(config)
                .AddSingleton<ConfigureGuildService>()
                .AddSingleton<ConfigurePresenceService>()
                .AddSingleton<ConfigureReadyService>()
                .AddSingleton<ReminderService>()
                .AddSingleton<WeatherService>()
                .AddSingleton<BalanceService>()
                .AddSingleton<Random>()
                .AddSingleton(new GoogleGeocoder(config.Credentials.Google))
                .AddSingleton(new DarkSkyService(config.Credentials.DarkSky))
                .AddSingleton(new HttpClient {Timeout = TimeSpan.FromSeconds(5)})
                // Discord Client
                .AddSingleton(client)
                // Discord Command Service
                .AddSingleton(commandService)
                .AddSingleton<CommandHandler>()
                .AddSingleton<InteractiveService>()
                // Database Contexts
                .AddDbContext<CoreContext>()
                .AddDbContext<UserContext>()
                // Logging
                .AddSingleton<LoggingService>()
                .AddLogging()
                // Memory Cache
                .AddSingleton<CacheHelper>()
                .AddMemoryCache();
            var services = collection.BuildServiceProvider();
            await ConfigureServicesAsync(services);
            return services;
        }

        private static async Task ConfigureServicesAsync(IServiceProvider services)
        {
            await services.GetRequiredService<UserContext>().Database.MigrateAsync();
            await services.GetRequiredService<CoreContext>().Database.MigrateAsync();
            services.GetRequiredService<LoggingService>();
            services.GetRequiredService<ConfigureGuildService>();
            services.GetRequiredService<ConfigurePresenceService>();
            services.GetRequiredService<ConfigureReadyService>();
            await services.GetRequiredService<CommandHandler>().InitAsync();
        }

        private static Task<ConfigModel> ConfigureSettingsAsync()
        {
            const string config = "config.json";
            if (!File.Exists(config))
            {
                string text = JsonConvert.SerializeObject(new ConfigModel(), Formatting.Indented);
                File.WriteAllText(config, text);
            }
            return Task.FromResult(JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(config)));
        }
    }
}