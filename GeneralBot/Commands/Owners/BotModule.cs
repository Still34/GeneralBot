using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Preconditions;
using GeneralBot.Results;

namespace GeneralBot.Commands.Admin
{
    [Group("bot")]
    [Summary("Bot-specific Settings")]
    [Remarks("Bot settings for owners.")]
    [RequireOwners]
    public class BotModule : ModuleBase<SocketCommandContext>
    {
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
    }
}