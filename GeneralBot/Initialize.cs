using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands;
using GeneralBot.Databases.Context;
using GeneralBot.Models;
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
            collection.AddSingleton(client);
            var commandSerivce = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
            commandSerivce.AddTypeReader<GuildPermissionTypeReader>(new GuildPermissionTypeReader());
            collection.AddDbContext<CoreContext>();
            collection.AddSingleton(commandSerivce);
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<LogService>();
            collection.AddSingleton<GuildConfigureService>();
            collection.AddSingleton<LatencyUpdatedHandler>();
            collection.AddSingleton(ConfigureSettings());
            collection.AddMemoryCache();
            collection.AddLogging();
            using (var db = new CoreContext())
            {
                await db.Database.MigrateAsync();
            }
            var services = collection.BuildServiceProvider();
            await ConfigureServices(services);
            return services;
        }

        private static async Task ConfigureServices(IServiceProvider services)
        {
            services.GetRequiredService<LogService>();
            services.GetRequiredService<GuildConfigureService>();
            services.GetRequiredService<LatencyUpdatedHandler>();
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