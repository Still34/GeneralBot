using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectoryMaid.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralBot.Commands.Results;
using GeneralBot.Extensions;
using GeneralBot.Extensions.Helpers;
using GeneralBot.Models.Database.UserSettings;
using GeneralBot.Preconditions;
using Humanizer;

namespace GeneralBot.Commands.User
{
    [Group("remind")]
    [Alias("remember", "reminder", "remindme")]
    [Summary("Remind Commands")]
    [Remarks("Forget things often? Remind yourself with these commands!")]
    public class RemindModule : ModuleBase<SocketCommandContext>
    {
        public ReminderService ReminderService { get; set; }
        public IUserRepository UserRepository { get; set; }

        [Command("remove")]
        [Summary("Remove the next reminder for yourself")]
        [Priority(10)]
        public async Task<RuntimeResult> RemoveRemindAsync()
        {
            var entry = UserRepository.GetReminders(Context.User)
                .OrderBy(x => x.Time)
                .FirstOrDefault();
            if (entry == null)
                return CommandRuntimeResult.FromError("You do not have a reminder set yet!");
            await UserRepository.RemoveReminderAsync(entry);
            return CommandRuntimeResult.FromSuccess(
                $@"Removed the reminder ""{entry.Content}"" that was planned at {entry.Time}.");
        }

        [Command("snooze")]
        [Summary("Snoozes the next reminder")]
        [Priority(5)]
        public async Task<RuntimeResult> SnoozeReminderAsync(
            [Summary("Time")] TimeSpan dateTimeParsed)
        {
            var entry = UserRepository.GetReminders(Context.User)
                .OrderBy(x => x.Time)
                .FirstOrDefault();
            if (entry == null) return CommandRuntimeResult.FromError("You do not have a reminder set yet!");
            entry.Time = entry.Time.Add(dateTimeParsed);
            await UserRepository.SaveRepositoryAsync();
            return CommandRuntimeResult.FromSuccess(
                $"Your next reminder '{entry.Content}' has been delayed for {dateTimeParsed.Humanize()}!");
        }

        [Command("list")]
        [Summary("List the remaining reminders for yourself")]
        [Priority(4)]
        public async Task<RuntimeResult> ListRemindersAsync()
        {
            var entries = UserRepository.GetReminders(Context.User)
                .OrderBy(x => x.Time);
            if (!entries.Any()) return CommandRuntimeResult.FromError("You do not have a reminder set yet!");

            var sb = new StringBuilder();
            var embed = ReminderService.GetReminderEmbed("");
            var userTimeOffset = GetUserTimeOffset(Context.User);
            if (userTimeOffset > TimeSpan.Zero)
                embed.WithFooter(x => x.Text = $"Converted to {Context.User.GetFullnameOrDefault()}'s timezone.");
            foreach (var entry in entries)
            {
                var time = entry.Time.ToOffset(userTimeOffset);
                sb.AppendLine($"**{time} ({entry.Time.Humanize()})**");
                string channelname = Context.Guild.GetChannel(entry.ChannelId)?.Name ?? "Unknown Channel";
                sb.AppendLine($@"   '{entry.Content}' at #{channelname}");
                sb.AppendLine();
            }
            await ReplyAsync("", embed: embed.WithDescription(sb.ToString()));
            return CommandRuntimeResult.FromSuccess();
        }

        [Command]
        [Summary("Set a reminder")]
        [Priority(3)]
        public async Task<RuntimeResult> RemindUserAsync(
            [Summary("Time")] TimeSpan dateTimeParsed,
            [Summary("Content")] [Remainder] string remindContent) =>
            await RemindAsync(Context.User, DateTimeOffset.Now.Add(dateTimeParsed), remindContent);

        [Command]
        [Summary("Set a reminder for another user")]
        [Priority(2)]
        [RequireModerator]
        public async Task<RuntimeResult> RemindOtherUserAsync(
            [Summary("User")] SocketUser user,
            [Summary("Time")] TimeSpan dateTimeParsed,
            [Summary("Content")] [Remainder] string remindContent) =>
            await RemindAsync(user, DateTimeOffset.Now.Add(dateTimeParsed), remindContent);

        [Command]
        [Summary("Set a reminder")]
        [Priority(1)]
        public async Task<RuntimeResult> RemindUserAsync(
            [Summary("Date Time")] DateTimeOffset dateTime,
            [Summary("Content")] [Remainder] string remindContent) =>
            await RemindAsync(Context.User, dateTime, remindContent);

        [Command]
        [Summary("Set a reminder for another user")]
        [Priority(0)]
        [RequireModerator]
        public async Task<RuntimeResult> RemindOtherUserAsync(
            [Summary("User")] SocketUser user,
            [Summary("Date Time")] DateTimeOffset dateTime,
            [Summary("Content")] [Remainder] string remindContent) =>
            await RemindAsync(user, dateTime, remindContent);

        private async Task<CommandRuntimeResult> RemindAsync(SocketUser user, DateTimeOffset dateTime,
            string remindContent)
        {
            if (DateTimeOffset.Now > dateTime)
                return CommandRuntimeResult.FromError($"{dateTime} has already passed!");
            var userTimeOffset = GetUserTimeOffset(user);
            await ReminderService.AddReminderAsync(user, Context.Channel, dateTime, remindContent);
            await ReplyAsync(string.Empty,
                embed: EmbedHelper.FromSuccess()
                    .WithAuthor(new EmbedAuthorBuilder
                    {
                        IconUrl = Context.Client.CurrentUser.GetAvatarUrlOrDefault(),
                        Name = "New Reminder Set!"
                    })
                    .AddInlineField("Reminder", remindContent)
                    .AddInlineField("At", dateTime.ToOffset(userTimeOffset)));
            return CommandRuntimeResult.FromSuccess();
        }

        // TODO: Implement TimezoneDb API.
        private TimeSpan GetUserTimeOffset(SocketUser user) => TimeSpan.Zero;
    }
}