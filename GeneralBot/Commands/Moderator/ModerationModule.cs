using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Preconditions;
using GeneralBot.Results;
using System.Collections.Generic;
using System.Linq;

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

        [Group("purge"), RequireContext(ContextType.Guild)]
        [Alias("clean")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireModerator]
        [Remarks("Clean messages that meet the criteria.")]
        public class CleanModule : ModuleBase
        {

            [Command("all")]
            public async Task<RuntimeResult> CleanAllAsync(int amount = 25)
            {
                var messages = await GetMessageAsync(amount);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count().ToString())} message(s)!");
            }

            [Command("user")]
            public async Task<RuntimeResult> CleanUserAsync(SocketUser user, int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Author.Id == user.Id);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count().ToString())} message(s) from user {Format.Bold(user.Mention)}!");
            }

            [Command("bots")]
            public async Task<RuntimeResult> CleanBotsAsync(int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Author.IsBot);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count().ToString())} message(s) from bots!");
            }

            [Command("contains")]
            public async Task<RuntimeResult> CleanContainsAsync(string text, int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Content.ToLower().Contains(text.ToLower()));
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count().ToString())} message(s) containing {text}!");
            }

            [Command("attachments")]
            public async Task<RuntimeResult> CleanAttachmentsAsync(int amount = 25)
            {
                var messages = (await GetMessageAsync(amount)).Where(x => x.Attachments.Count() != 0);
                await DeleteMessagesAsync(messages);
                return CommandRuntimeResult.FromSuccess($"Deleted {Format.Bold(messages.Count().ToString())} message(s) containing attachments!");
            }

            private Task<IEnumerable<IMessage>> GetMessageAsync(int count)
                => Context.Channel.GetMessagesAsync(count).Flatten();

            private Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
                => Context.Channel.DeleteMessagesAsync(messages);

        }
    }
}