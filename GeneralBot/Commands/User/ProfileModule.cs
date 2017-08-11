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
        public IUserRepository UserRepository { get; set; }

        [Command("balance")]
        [Summary("Shows the specified user's balance.")]
        public async Task<RuntimeResult> BalanceAsync(SocketUser user = null)
        {
            var targetUser = user ?? Context.User;
            var record = await UserRepository.GetOrCreateProfileAsync(targetUser);
            return CommandRuntimeResult.FromInfo(
                $"{targetUser.Mention}'s current balance is {record.Balance}{Config.CurrencySymbol}");
        }

        [Group("summary")]
        public class Summary : ModuleBase<SocketCommandContext>
        {
            public IUserRepository UserRepository { get; set; }

            [Command]
            public async Task<RuntimeResult> CheckSummaryAsync(SocketUser user = null)
            {
                var targetUser = user ?? Context.User;
                var record = await UserRepository.GetOrCreateProfileAsync(targetUser);
                return CommandRuntimeResult.FromInfo($"Current Summary: {Format.Bold(record.Summary)}");
            }

            [Command("set")]
            public async Task<RuntimeResult> SetSummaryAsync([Remainder] string summary)
            {
                var record = await UserRepository.GetOrCreateProfileAsync(Context.User);
                record.Summary = summary;
                await UserRepository.SaveRepositoryAsync();
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
                public IUserRepository UserRepository { get; set; }

                [Command]
                public async Task<RuntimeResult> CheckSteamAsync(SocketUser user = null)
                {
                    var targetUser = user ?? Context.User;
                    var record = UserRepository.GetGame(targetUser);
                    if (record == null || record.SteamId == 0)
                        return CommandRuntimeResult.FromError("User hasn't setup their steam profile yet!");

                    var profile = await SteamService.GetProfileAsync(record.SteamId);
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
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User);
                    record.SteamId = id;
                    await UserRepository.SaveRepositoryAsync();
                    return CommandRuntimeResult.FromSuccess(
                        $"Successfully set steam to {Format.Bold((await SteamService.GetProfileAsync(id))?.CustomURL ?? id.ToString())}");
                }
            }

            [Group("battletag")]
            [Alias("bnet", "blizzard", "battlenet", "battle-net", "battle-tag", "btag")]
            public class BattleNet : ModuleBase<SocketCommandContext>
            {
                public IUserRepository UserRepository { get; set; }

                [Command]
                public Task<RuntimeResult> CheckBattleTagAsync(SocketUser user = null)
                {
                    var targetUser = user ?? Context.User;
                    var record = UserRepository.GetGame(targetUser);
                    return Task.FromResult<RuntimeResult>(record?.BattleTag == null
                        ? CommandRuntimeResult.FromError("User hasn't setup their BattleTag yet!")
                        : CommandRuntimeResult.FromInfo(
                            $"{targetUser.Mention}'s BattleTag: {Format.Bold(record.BattleTag)}")
                    );
                }

                [Command("set")]
                public async Task<RuntimeResult> SetBattleTagAsync([Remainder] string username)
                {
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User);
                    record.BattleTag = username;
                    await UserRepository.SaveRepositoryAsync();
                    return CommandRuntimeResult.FromSuccess(
                        $"Successfully set BattleTag to {Format.Bold(username)}");
                }

                //TODO: Add overwatch statistics based on battletag.
            }

            [Group("riot")]
            [Alias("league", "league-of-legends", "leagueoflegends", "lol")]
            public class Riot : ModuleBase<SocketCommandContext>
            {
                public IUserRepository UserRepository { get; set; }

                [Command]
                public Task<RuntimeResult> CheckRiotAsync(SocketUser user = null)
                {
                    var targetUser = user ?? Context.User;
                    var record = UserRepository.GetGame(targetUser);
                    return Task.FromResult<RuntimeResult>(record?.RiotId == null
                        ? CommandRuntimeResult.FromError("User hasn't setup their Riot Id yet!")
                        : CommandRuntimeResult.FromInfo(
                            $"{targetUser.Mention}'s Riot Id: {Format.Bold(record.RiotId)}"));
                }

                [Command("set")]
                public async Task<RuntimeResult> SetRiotAsync([Remainder] string username)
                {
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User);
                    record.RiotId = username;
                    await UserRepository.SaveRepositoryAsync();
                    return CommandRuntimeResult.FromSuccess(
                        $"Successfully set Riot Id to {Format.Bold(username)}");
                }

                //TODO: Add league stats based on riot acc.    
            }

            [Group("nintendo")]
            //[Alias("switch", "3ds", "friendcode")]
            public class Nintendo : ModuleBase<SocketCommandContext>
            {
                public IUserRepository UserRepository { get; set; }
                //TODO: Impl Switch/3DS variants.
                [Command]
                public Task<RuntimeResult> CheckFriendCodeAsync(SocketUser user = null)
                {
                    var targetUser = user ?? Context.User;
                    var record = UserRepository.GetGame(targetUser);
                    return Task.FromResult<RuntimeResult>(record?.NintendoFriendCode == null
                        ? CommandRuntimeResult.FromError("User hasn't setup their Nintendo Friend Code yet!")
                        : CommandRuntimeResult.FromInfo(
                            $"{targetUser.Mention}'s Nintendo Friend Code: {Format.Bold(record.NintendoFriendCode)}"));
                }

                [Command("set")]
                public async Task<RuntimeResult> SetFriendCodeAsync([Remainder] string username)
                {
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User);
                    record.NintendoFriendCode = username;
                    await UserRepository.SaveRepositoryAsync();
                    return CommandRuntimeResult.FromSuccess(
                        $"Successfully set Nintendo Friend Code to {Format.Bold(username)}");
                }
            }
        }
    }
}