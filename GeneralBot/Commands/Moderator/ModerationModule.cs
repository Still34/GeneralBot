using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Extensions;
using GeneralBot.Preconditions;
using GeneralBot.Results;

namespace GeneralBot.Commands.Moderator
{
    [Group("mod")]
    [Summary("Moderator Commands")]
    [Remarks("Comamnds used for server moderation.")]
    [RequireContext(ContextType.Guild)]
    [RequireModerator]
    public class ModerationModule : ModuleBase<CustomCommandContext>
    {
        [Command("kick")]
        [Summary("Kicks the selected user with specified reason, if any.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
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
        [RequireUserPermission(GuildPermission.BanMembers)]
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
        [RequireUserPermission(GuildPermission.BanMembers)]
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
        [RequireUserPermission(GuildPermission.BanMembers)]
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
        [RequireUserPermission(GuildPermission.BanMembers)]
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
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        public async Task<RuntimeResult> NicknameChange([RequireHierarchy] SocketGuildUser user, [Remainder] string nickname)
        {
            await user.ModifyAsync(x => x.Nickname = nickname);
            return CommandRuntimeResult.FromSuccess($"Successfully changed {user}'s name to {nickname}.");
        }

        [Group("purge")]
        [RequireContext(ContextType.Guild)]
        [Alias("clean")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireModerator]
        [Remarks("Clean messages that meet the criteria.")]
        public class CleanModule : ModuleBase<CustomCommandContext>
        {
            private const string MessagesNotFound = "Found 0 messages!";

            [Command("all")]
            public async Task<RuntimeResult> CleanAllAsync(int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).ToList();
                if (messages.Count == 0) return CommandRuntimeResult.FromError(MessagesNotFound);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count.ToString())} message(s)!");
            }

            [Command("user")]
            public async Task<RuntimeResult> CleanUserAsync(SocketUser user, int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Author.Id == user.Id).ToList();
                if (messages.Count == 0) return CommandRuntimeResult.FromError(MessagesNotFound);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count.ToString())} message(s) from user {Format.Bold(user.Mention)}!");
            }

            [Command("bots")]
            public async Task<RuntimeResult> CleanBotsAsync(int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Author.IsBot).ToList();
                if (messages.Count == 0) return CommandRuntimeResult.FromError(MessagesNotFound);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count.ToString())} message(s) from bots!");
            }

            [Command("contains")]
            public async Task<RuntimeResult> CleanContainsAsync(string text, int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Content.ContainsCaseInsensitive(text)).ToList();
                if (messages.Count == 0) return CommandRuntimeResult.FromError(MessagesNotFound);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count.ToString())} message(s) containing {text}!");
            }

            [Command("attachments")]
            public async Task<RuntimeResult> CleanAttachmentsAsync(int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Attachments.Count > 0).ToList();
                if (messages.Count == 0) return CommandRuntimeResult.FromError(MessagesNotFound);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count.ToString())} message(s) containing attachments!");
            }

            private async Task<IEnumerable<IMessage>> GetMessageAsync(int count)
                => await Context.Channel.GetMessagesAsync(count).Flatten();

            private async Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
                => await Context.Channel.DeleteMessagesAsync(messages);
        }
    }
}