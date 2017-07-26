using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Preconditions;
using GeneralBot.Results;

namespace GeneralBot.Commands.Moderator
{
    [Group("mod")]
    [Summary("Moderator Commands")]
    [Remarks("Comamnds used for server moderation.")]
    [RequireContext(ContextType.Guild)]
    [RequireModerator]
    public class ModerationModule : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        [Summary("Kicks the selected user with specified reason, if any.")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task<RuntimeResult> KickUserAsync(
            [RequireHierarchy] SocketGuildUser user,
            [Remainder] string reason = null)
        {
            await user.KickAsync(reason);
            return CommandRuntimeResult.FromSuccess($"User {user} has been kicked from the server.");
        }

        [Command("ban")]
        [Priority(1)]
        [Summary("Bans the selected user with specified reason, if any.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> BanUserAsync(
            [RequireHierarchy] SocketGuildUser user,
            int days = 0,
            [Remainder] string reason = null)
        {
            await Context.Guild.AddBanAsync(user, days, reason);
            return CommandRuntimeResult.FromSuccess($"User {user} has been banned from the server.");
        }

        [Command("ban")]
        [Priority(0)]
        [Summary("Bans the selected user with the specified ID and reason, if any.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> BanUserAsync(
            [RequireHierarchy] ulong userId,
            int days = 0,
            [Remainder] string reason = null)
        {
            await Context.Guild.AddBanAsync(userId, days, reason);
            return CommandRuntimeResult.FromSuccess($"User {userId} has been banned from the server.");
        }

        [Command("softban")]
        [Priority(1)]
        [Summary("Bans the user and then unbans. Useful for purging content for the targetted user.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> SoftBanAsync(
            [RequireHierarchy] SocketGuildUser user,
            int days = 0,
            [Remainder] string reason = null)
        {
            await Context.Guild.AddBanAsync(user, days, reason);
            await Context.Guild.RemoveBanAsync(user);
            return CommandRuntimeResult.FromSuccess($"User {user} has been banned from the server.");
        }

        [Command("softban")]
        [Priority(0)]
        [Summary("Bans the user and then unbans. Useful for purging content for the targetted user.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task<RuntimeResult> SoftBanAsync(
            [RequireHierarchy] ulong userId,
            int days = 0,
            [Remainder] string reason = null)
        {
            await Context.Guild.AddBanAsync(userId, days, reason);
            await Context.Guild.RemoveBanAsync(userId);
            return CommandRuntimeResult.FromSuccess($"User {userId} has been banned from the server.");
        }

        [Command("nickname")]
        [Summary("Changes the nickname for the targeted user.")]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task<RuntimeResult> NicknameChange([RequireHierarchy] SocketGuildUser user, [Remainder] string nickname)
        {
            await user.ModifyAsync(x => x.Nickname = nickname);
            return CommandRuntimeResult.FromSuccess($"Successfully changed {user}'s name to {nickname}.");
        }
    }
}