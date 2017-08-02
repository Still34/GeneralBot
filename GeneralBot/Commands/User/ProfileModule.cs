using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Models;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.UserSettings;
using GeneralBot.Results;

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
            var dbEntry = UserSettings.Profiles.SingleOrDefault(x => x.UserId == targetUser.Id) ?? UserSettings.Profiles
                              .Add(new Profile {UserId = targetUser.Id, LastMessage = Context.Message.Timestamp})
                              .Entity;
            return Task.FromResult<RuntimeResult>(
                CommandRuntimeResult.FromSuccess(
                    $"{targetUser.Mention}'s current balance is {dbEntry.Balance}{Config.CurrencySymbol}"));
        }

        [Group("summary")]
        public class Summary : ModuleBase<SocketCommandContext>
        {
            public UserContext UserSettings { get; set; }

            [Command]
            public Task<RuntimeResult> CheckSummary(SocketUser user = null)
            {
                var targetUser = user ?? Context.User;
                var dbEntry = UserSettings.Profiles.SingleOrDefault(x => x.UserId == targetUser.Id) ?? UserSettings
                                  .Profiles.Add(new Profile
                                  {
                                      UserId = targetUser.Id,
                                      LastMessage = Context.Message.Timestamp
                                  }).Entity;
                return Task.FromResult<RuntimeResult>(
                    CommandRuntimeResult.FromInfo($"Current Summary: {Format.Bold(dbEntry.Summary)}"));
            }

            [Command("set")]
            public async Task<RuntimeResult> SetSummary([Remainder] string summary)
            {
                var dbEntry = UserSettings.Profiles.SingleOrDefault(x => x.UserId == Context.User.Id) ?? UserSettings
                                  .Profiles.Add(new Profile
                                  {
                                      UserId = Context.User.Id,
                                      LastMessage = Context.Message.Timestamp
                                  }).Entity;
                dbEntry.Summary = summary;
                UserSettings.Add(dbEntry);
                await UserSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Successfully set summary to {Format.Bold(summary)}");
            }
        }
    }
}