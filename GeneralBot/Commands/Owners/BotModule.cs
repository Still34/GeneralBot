using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Preconditions;
using GeneralBot.Results;
using GeneralBot.Services;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace GeneralBot.Commands.Admin
{
    [Group("bot")]
    [Summary("Bot-specific Settings")]
    [Remarks("Bot settings for owners.")]
    [RequireOwners]
    public class BotModule : ModuleBase<SocketCommandContext>
    {
        public CacheHelper _cacheHelper;
        public IMemoryCache _cache;

        public BotModule(CacheHelper cacheHelper, IMemoryCache cache)
        {
            _cacheHelper = cacheHelper;
            _cache = cache;
        }

        [Command("username")]
        [Summary("Changes the bot's username.")]
        public async Task<RuntimeResult> ConfigUsername([Remainder] string username)
        {
            await Context.Client.CurrentUser.ModifyAsync(x => { x.Username = username; });
            return CommandRuntimeResult.FromSuccess($"Successfully changed username to {Format.Bold(username)}.");
        }

        [Command("game")]
        [Summary("Changes the bot's playing status.")]
        public async Task<RuntimeResult> ConfigGame([Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
            return CommandRuntimeResult.FromSuccess($"Successfully changed game to {Format.Bold(game)}.");
        }

        [Command("throw")]
        public Task ThrowException()
        {
            throw new InvalidOperationException("This is a test error.");
        }

        [Command("status")]
        [Summary("Changes the bot's status.")]
        public async Task<RuntimeResult> ConfigStatus(string status)
        {
            switch (status.ToLower())
            {
                case "online":
                    await Context.Client.SetStatusAsync(UserStatus.Online);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Online")}.");
                case "offline":
                    await Context.Client.SetStatusAsync(UserStatus.Offline);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Offline")}.");
                case "afk":
                    await Context.Client.SetStatusAsync(UserStatus.AFK);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("AFK")}.");
                case "idle":
                    await Context.Client.SetStatusAsync(UserStatus.Idle);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Idle")}.");
                case "dnd":
                    await Context.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Do Not Disturb")}.");
                case "invisible":
                    await Context.Client.SetStatusAsync(UserStatus.Invisible);
                    return CommandRuntimeResult.FromSuccess($"Successfully changed status to {Format.Bold("Invisible")}.");
                default:
                    return CommandRuntimeResult.FromError($"{Format.Bold(status)} is not a valid status.");
            }
        }

        [Command("test")]
        public async Task<RuntimeResult> Test(string key, string value)
        {
           return CommandRuntimeResult.FromSuccess((await _cacheHelper.TryGetValueSet<string, string>(key, value, TimeSpan.FromMinutes(1))));
        }

        [Command("test2")]
        public async Task<RuntimeResult> Test(string key)
        {
            if(_cache.TryGetValue(key, out string value))
            {
                return CommandRuntimeResult.FromSuccess(value);
            }
            else
            {
                return CommandRuntimeResult.FromError($"No value found for {key} in the cache!");
            }

        }
    }
}