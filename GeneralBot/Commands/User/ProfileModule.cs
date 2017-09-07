using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.UserSettings;
using GeneralBot.Services;
using ImageSharp;
using ImageSharp.Drawing;
using SixLabors.Fonts;
using SixLabors.Primitives;
using Image = ImageSharp.Image;

namespace GeneralBot.Commands.User
{
    [Group("profile")]
    public class ProfileModule : ModuleBase<SocketCommandContext>
    {
        public ConfigModel Config { get; set; }
        public HttpClient HttpClient { get; set; }
        public BalanceService BalanceService { get; set; }
        public IUserRepository UserRepository { get; set; }
        private IDisposable TypingDisposable { get; set; }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            TypingDisposable = Context.Channel.EnterTypingState();
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            TypingDisposable.Dispose();
        }

        [Command]
        public async Task<RuntimeResult> ProfileAsync(SocketUser user = null)
        {
            //User Data
            var targetUser = user ?? Context.User;
            var record = await UserRepository.GetOrCreateProfileAsync(targetUser).ConfigureAwait(false);

            using (var profileBase = Image.Load("Resources/Profile/base.png"))
            using (var avatarFrame = Image.Load("Resources/Profile/avatarFrame.png"))
            using (var badge = Image.Load("Resources/Profile/badge.png"))
            using (var avatarStream = await WebHelper.GetFileStreamAsync(HttpClient, new Uri(targetUser.GetAvatarUrl())).ConfigureAwait(false))
            using (var finalImage = new MemoryStream())
            using (var avatar = Image.Load(avatarStream))
            {
                //Fonts & text rendering
                var robotoItalic = new FontCollection().Install("Resources/Profile/Fonts/Roboto-Italic.ttf");
                var roboto = new FontCollection().Install("Resources/Profile/Fonts/Roboto-Regular.ttf");
                var textOptions = new TextGraphicsOptions { HorizontalAlignment = HorizontalAlignment.Center };

                //Actual Image
                var final = profileBase.DrawImage(avatar.Resize(540, 540), new Size(), new Point(935, 134),
                        GraphicsOptions.Default)
                    .DrawImage(avatarFrame, new Size(), new Point(0, 0), GraphicsOptions.Default)
                    .DrawText(targetUser.Username, robotoItalic.CreateFont(130), new Rgba32(106, 161, 196),
                        new Point(profileBase.Width / 2, 690), textOptions)
                    .DrawText($"Member Since {targetUser.CreatedAt:M/d/yy}", robotoItalic.CreateFont(73),
                        new Rgba32(138, 145, 153),
                        new Point(profileBase.Width / 2, 840), textOptions)
                    .DrawText($"Balance: {record.Balance}", roboto.CreateFont(82), new Rgba32(138, 145, 153),
                        new Point(profileBase.Width / 2, 930), textOptions)
                    .DrawText("About Me:", robotoItalic.CreateFont(90), new Rgba32(106, 161, 196), new Point(340, 1130),
                        TextGraphicsOptions.Default)
                    .DrawText(Regex.Replace(record.Summary, ".{55}", "$0\n"), roboto.CreateFont(70), new Rgba32(138, 145, 153), new Point(340, 1250),
                        TextGraphicsOptions.Default);

                //Verified Badge
                //TODO: Add some kind of verification system/reputation system?
                if (Config.Owners.Contains(targetUser.Id))
                    final.DrawImage(badge, new Size(), new Point(0, 0), GraphicsOptions.Default).Resize(500, 500).SaveAsPng(finalImage);
                else
                    final.Resize(500, 500).SaveAsPng(finalImage);

                //Sending the image
                finalImage.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(finalImage, "profile.png", $"Profile card for `{targetUser}`").ConfigureAwait(false);
            }
            return CommandRuntimeResult.FromSuccess();
        }

        [Group("balance")]
        public class BalanceModule : ModuleBase<SocketCommandContext>
        {
            public ConfigModel Config { get; set; }
            public IUserRepository UserRepository { get; set; }
            public BalanceService BalanceService { get; set; }

            [Command]
            [Summary("Shows the specified user's balance.")]
            public async Task<RuntimeResult> BalanceAsync(SocketUser user = null)
            {
                var targetUser = user ?? Context.User;
                var record = await UserRepository.GetOrCreateProfileAsync(targetUser).ConfigureAwait(false);
                var wealthLevel = BalanceService.GetLevel(record.Balance);
                var builder = new EmbedBuilder
                    {
                        Author = new EmbedAuthorBuilder
                        {
                            Name = targetUser.GetFullnameOrDefault(),
                            IconUrl = targetUser.GetAvatarUrlOrDefault()
                        },
                        Color = ColorHelper.GetRandomColor()
                    }.AddInlineField("Balance",
                        $"`{record.Balance}`{Config.CurrencySymbol}\nRank: `{NumberHelper.AddOrdinal(BalanceService.GetRank(record.Balance))}`")
                    .AddInlineField("Wealth Level",
                        $"Current: `{wealthLevel}`\nNext: `{wealthLevel + 1}` (`{record.Balance}`/`{BalanceService.GetBalanceForLevel(wealthLevel + 1)}`)");
                await ReplyAsync("", embed: builder.Build()).ConfigureAwait(false);
                return CommandRuntimeResult.FromSuccess();
            }

            [Command("leaderboard")]
            [Alias("top")]
            [Summary("Shows the balance leaderboard.")]
            public async Task<RuntimeResult> LeaderboardAsync()
            {
                var richest = UserRepository.GetProfiles().OrderByDescending(x => x.Balance).Take(5);
                var sb = new StringBuilder();
                foreach (var user in richest)
                {
                    sb.AppendLine(
                        $"{Format.Bold(NumberHelper.AddOrdinal(BalanceService.GetRank(user.Balance)))}: " +
                        $"{Format.Code(Context.Client.GetUser(user.UserId).ToString())} - " +
                        $"{user.Balance} {Config.CurrencySymbol}");
                }
                await ReplyAsync("", embed: new EmbedBuilder
                {
                    Title = "Balance Leaderboard:",
                    Color = ColorHelper.GetRandomColor(),
                    Description = sb.ToString()
                }.Build());
                return CommandRuntimeResult.FromSuccess();
            }
            
        }

        [Group("summary")]
        public class Summary : ModuleBase<SocketCommandContext>
        {
            public IUserRepository UserRepository { get; set; }

            [Command]
            public async Task<RuntimeResult> CheckSummaryAsync(SocketUser user = null)
            {
                var targetUser = user ?? Context.User;
                var record = await UserRepository.GetOrCreateProfileAsync(targetUser).ConfigureAwait(false);
                return CommandRuntimeResult.FromInfo($"Current Summary: {Format.Bold(record.Summary)}");
            }

            [Command("set")]
            public async Task<RuntimeResult> SetSummaryAsync([Remainder] string summary)
            {
                if (summary.Length > 500)
                    return CommandRuntimeResult.FromError("Your summary needs to be shorter than 500 characters!");
                var record = await UserRepository.GetOrCreateProfileAsync(Context.User).ConfigureAwait(false);
                record.Summary = summary;
                await UserRepository.SaveRepositoryAsync().ConfigureAwait(false);
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

                    var profile = await SteamService.GetProfileAsync(record.SteamId).ConfigureAwait(false);
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
                    await ReplyAsync("", embed: builder.Build()).ConfigureAwait(false);
                    return CommandRuntimeResult.FromSuccess();
                }

                [Command("set")]
                public async Task<RuntimeResult> SetSteamAsync([Remainder] string username)
                {
                    ulong id = await SteamService.GetIdFromVanityAsync(username).ConfigureAwait(false);
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User).ConfigureAwait(false);
                    record.SteamId = id;
                    await UserRepository.SaveRepositoryAsync().ConfigureAwait(false);
                    return CommandRuntimeResult.FromSuccess(
                        $"Successfully set steam to {Format.Bold((await SteamService.GetProfileAsync(id).ConfigureAwait(false))?.CustomURL ?? id.ToString())}");
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
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User).ConfigureAwait(false);
                    record.BattleTag = username;
                    await UserRepository.SaveRepositoryAsync().ConfigureAwait(false);
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
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User).ConfigureAwait(false);
                    record.RiotId = username;
                    await UserRepository.SaveRepositoryAsync().ConfigureAwait(false);
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
                    var record = await UserRepository.GetOrCreateGameAsync(Context.User).ConfigureAwait(false);
                    record.NintendoFriendCode = username;
                    await UserRepository.SaveRepositoryAsync().ConfigureAwait(false);
                    return CommandRuntimeResult.FromSuccess(
                        $"Successfully set Nintendo Friend Code to {Format.Bold(username)}");
                }
            }
        }
    }
}