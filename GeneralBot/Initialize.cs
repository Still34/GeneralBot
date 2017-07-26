using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands;
using GeneralBot.Databases.Context;
using GeneralBot.Models;
using GeneralBot.Models.Context;
using GeneralBot.Services;
using GeneralBot.Typereaders;
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
            var commandSerivce = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
            commandSerivce.AddTypeReader<GuildPermissionTypeReader>(new GuildPermissionTypeReader());
            collection
                // Database Contexts
                .AddDbContext<CoreContext>()
                .AddDbContext<UserContext>()
                // Discord Client
                .AddSingleton(client)
                // Discord Command Service
                .AddSingleton(commandSerivce)
                .AddSingleton<CommandHandler>()
                // Logging
                .AddSingleton<LoggingService>()
                .AddLogging()
                // Misc Services / Configs
                .AddSingleton<GuildConfigureService>()
                .AddSingleton<StatusConfigureService>()
                .AddSingleton(ConfigureSettings())
                .AddSingleton(new Random())
                // Memory Cache
                .AddSingleton<CacheHelper>()
                .AddMemoryCache();
            var services = collection.BuildServiceProvider();
            await ConfigureServices(services);
            return services;
        }

        private static async Task ConfigureServices(IServiceProvider services)
        {
            await services.GetRequiredService<UserContext>().Database.MigrateAsync();
            await services.GetRequiredService<CoreContext>().Database.MigrateAsync();
            services.GetRequiredService<LoggingService>();
            services.GetRequiredService<GuildConfigureService>();
            services.GetRequiredService<StatusConfigureService>();
            await services.GetRequiredService<CommandHandler>().InitAsync();
        }

        private static ConfigModel ConfigureSettings()
        {
            const string config = "config.json";
            if (!File.Exists(config))
            {
                string text = JsonConvert.SerializeObject(new ConfigModel(), Formatting.Indented);
                File.WriteAllText(config, text);
            }
            return JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(config));
        }
    }
}