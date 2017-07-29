using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Models;
using GeneralBot.Models.Context;
using GeneralBot.Results;
using ImageSharp;
using GeneralBot.Extensions;
using System.Net.Http;
using SixLabors.Primitives;
using SixLabors.Fonts;
using Discord.WebSocket;
using Discord;

namespace GeneralBot.Commands.User
{
    [Group("profile")]
    public class ProfileModule : ModuleBase<SocketCommandContext>
    {
        public UserContext UserSettings { get; set; }
        public ConfigModel Config { get; set; }
        public HttpClient HttpClient { get; set; }

        [Command("balance")]
        [Summary("Shows the specified user's balance.")]
        public Task<RuntimeResult> Balance(SocketUser user = null)
        {
            var targetUser = user ?? Context.User;
            var dbEntry = UserSettings.Profile.SingleOrDefault(x => x.UserId == targetUser.Id) ?? UserSettings.Profile.Add(new Profile { UserId = targetUser.Id, LastMessage = Context.Message.Timestamp }).Entity;
            return Task.FromResult<RuntimeResult>(CommandRuntimeResult.FromSuccess($"{targetUser.Mention}'s current balance is {dbEntry.Balance}{Config.CurrencySymbol}"));
        }

        [Group("summary")]
        public class Summary : ModuleBase<SocketCommandContext>
        {
            public UserContext UserSettings { get; set; }

            [Command()]
            public Task<RuntimeResult> CheckSummary(SocketUser user = null)
            {
                var targetUser = user ?? Context.User;
                var dbEntry = UserSettings.Profile.SingleOrDefault(x => x.UserId == targetUser.Id) ?? UserSettings.Profile.Add(new Profile { UserId = targetUser.Id, LastMessage = Context.Message.Timestamp }).Entity;
                return Task.FromResult<RuntimeResult>(CommandRuntimeResult.FromInfo($"Current Summary: {Format.Bold(dbEntry.Summary)}"));
            }

            [Command("set")]
            public async Task<RuntimeResult> SetSummary([Remainder] string summary)
            {
                var dbEntry = UserSettings.Profile.SingleOrDefault(x => x.UserId == Context.User.Id) ?? UserSettings.Profile.Add(new Profile { UserId = Context.User.Id, LastMessage = Context.Message.Timestamp }).Entity;
                dbEntry.Summary = summary;
                UserSettings.Add(dbEntry);
                await UserSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Successfully set summary to {Format.Bold(summary)}");
            }
        }
    }
}