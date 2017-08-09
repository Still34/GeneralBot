using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.UserSettings;
using GeneralBot.Services;

namespace GeneralBot.Commands.User
{
    [Group("profile")]
    public class ProfileModule : ModuleBase<SocketCommandContext>
    {
        public ConfigModel Config { get; set; }
        public HttpClient HttpClient { get; set; }
        public UserContext UserSettings { get; set; }

        [Command("balance")]
        [Summary("Shows the specified user's balance.")]
        public Task<RuntimeResult> BalanceAsync(SocketUser user = null)
        {
            var targetUser = user ?? Context.User;
            var dbEntry = UserSettings.Profiles.SingleOrDefault(x => x.UserId == targetUser.Id) ??
                          UserSettings.Profiles.Add(new Profile {UserId = targetUser.Id}).Entity;
            return Task.FromResult<RuntimeResult>(
                CommandRuntimeResult.FromSuccess(
                    $"{targetUser.Mention}'s current balance is {dbEntry.Balance}{Config.CurrencySymbol}"));
        }

        [Group("summary")]
        public class Summary : ModuleBase<SocketCommandContext>
        {
            public UserContext UserSettings { get; set; }

            [Command]
            public Task<RuntimeResult> CheckSummaryAsync(SocketUser user = null)
            {
                var targetUser = user ?? Context.User;
                var dbEntry = UserSettings.Profiles.SingleOrDefault(x => x.UserId == targetUser.Id) ??
                              UserSettings.Profiles.Add(new Profile {UserId = targetUser.Id}).Entity;
                return Task.FromResult<RuntimeResult>(
                    CommandRuntimeResult.FromInfo($"Current Summary: {Format.Bold(dbEntry.Summary)}"));
            }

            [Command("set")]
            public async Task<RuntimeResult> SetSummaryAsync([Remainder] string summary)
            {
                var dbEntry = UserSettings.Profiles.SingleOrDefault(x => x.UserId == Context.User.Id) ??
                              UserSettings.Profiles.Add(new Profile {UserId = Context.User.Id}).Entity;
                dbEntry.Summary = summary;

                await UserSettings.SaveChangesAsync();
                return CommandRuntimeResult.FromSuccess($"Successfully set summary to {Format.Bold(summary)}");
            }
        }

        [Group("games")]
        [Alias("game", "g")]
        public class GamesModule : ModuleBase<SocketCommandContext>
        {
            [Group("steam")]
            public class Steam : ModuleBase<SocketCommandContext>
            {
                public SteamService SteamService { get; set; }
                public UserContext UserSettings { get; set; }

                [Command]
                public async Task<RuntimeResult> CheckSteamAsync(SocketUser user = null)
                {
                    var targetUser = user ?? Context.User;
                    var dbEntry = UserSettings.Games.SingleOrDefault(x => x.UserId == targetUser.Id);
                    if (dbEntry == null || dbEntry.SteamId == 0)
                        return CommandRuntimeResult.FromError("User hasn't setup their steam profile yet!");

                    var profile = await SteamService.GetProfileAsync(dbEntry.SteamId);
                    var builder = new EmbedBuilder
                        {
                            Author = new EmbedAuthorBuilder
                            {
                                Name = profile.CustomURL ?? profile.SteamID.ToString(),
                                IconUrl = profile.Avatar.AbsoluteUri
                            },
                            Description = profile.Summary,
                            Footer = new EmbedFooterBuilder
                            {
                                Text =
                                    $"Recently Played: {string.Join(", ", profile.MostPlayedGames.Select(x => x.Name))}"
                            },
                            Color = ColorHelper.GetRandomColor()
                        }
                        .AddInlineField("State:", profile.StateMessage)
                        .AddInlineField("Member Since:", profile.MemberSince)
                        .AddInlineField("Location:",
                            string.IsNullOrWhiteSpace(profile.Location) ? "Not Specified." : profile.Location)
                        .AddInlineField("Real Name:",
                            string.IsNullOrWhiteSpace(profile.RealName) ? "Not Specified." : profile.RealName)
                        .AddInlineField("VAC Banned?:", profile.IsVacBanned ? "Yes." : "No.");
                    await ReplyAsync("", embed: builder);
                    return CommandRuntimeResult.FromSuccess();
                }

                [Command("set")]
                public async Task<RuntimeResult> SetSteamAsync([Remainder] string username)
                {
                    ulong id = await SteamService.GetIdFromVanityAsync(username);
                    var dbEntry = UserSettings.Games.SingleOrDefault(x => x.UserId == Context.User.Id) ??
                                  UserSettings.Games.Add(new Games {UserId = Context.User.Id}).Entity;

                    dbEntry.SteamId = id;
                    UserSettings.Add(dbEntry);
                    await UserSettings.SaveChangesAsync();
                    return CommandRuntimeResult.FromSuccess(
                        $"Successfully set steam to {Format.Bold((await SteamService.GetProfileAsync(id))?.CustomURL ?? id.ToString())}");
                }
            }
        }
    }
}