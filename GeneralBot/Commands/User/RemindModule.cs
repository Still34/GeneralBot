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
using GeneralBot.Typereaders;
using Humanizer;

namespace GeneralBot.Commands.User
{
    [Group("remind")]
    [Alias("remember", "reminder", "remindme")]
    public class RemindModule : ModuleBase<SocketCommandContext>
    {
        public ReminderService ReminderService { get; set; }
        public UserContext UserContext { get; set; }

        [Command("remove")]
        [Summary("Remove the next reminder for yourself")]
        [Priority(10)]
        public async Task<RuntimeResult> RemoveRemind()
        {
            var entry = UserContext.Reminders.Where(x => x.UserId == Context.User.Id)
                .OrderBy(x => x.Time)
                .FirstOrDefault();
            if (entry == null)
                return CommandRuntimeResult.FromError("You do not have a reminder set yet!");
            UserContext.Remove(entry);
            await UserContext.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess(
                $@"Removed the reminder ""{entry.Content}"" that was planned at {entry.Time}.");
        }

        [Command("snooze")]
        [Summary("Snoozes the next reminder")]
        [Priority(5)]
        public async Task<RuntimeResult> SnoozeReminder(
            [Summary("Time")] [OverrideTypeReader(typeof(StringTimeSpanTypeReader))] TimeSpan dateTimeParsed)
        {
            var entry = UserContext.Reminders.Where(x => x.UserId == Context.User.Id)
                .OrderBy(x => x.Time)
                .FirstOrDefault();
            if (entry == null) return CommandRuntimeResult.FromError("You do not have a reminder set yet!");
            entry.Time = entry.Time.Add(dateTimeParsed);
            await UserContext.SaveChangesAsync();
            return CommandRuntimeResult.FromSuccess(
                $"Your next reminder '{entry.Content}' has been delayed for {dateTimeParsed.Humanize()}!");
        }

        [Command("list")]
        [Summary("List the remaining reminders for yourself")]
        [Priority(4)]
        public async Task<RuntimeResult> ListReminders()
        {
            var entries = UserContext.Reminders.Where(x => x.UserId == Context.User.Id)
                .OrderBy(x => x.Time);
            if (!entries.Any()) return CommandRuntimeResult.FromError("You do not have a reminder set yet!");

            var sb = new StringBuilder();
            var embed = ReminderService.GetReminderEmbed("");
            var userTimeOffset = await GetUserTimeOffset(Context.User);
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
        public async Task<RuntimeResult> RemindUser(
            [Summary("Time")] [OverrideTypeReader(typeof(StringTimeSpanTypeReader))] TimeSpan dateTimeParsed,
            [Summary("Content")] [Remainder] string remindContent) =>
            await Remind(Context.User, DateTimeOffset.Now.Add(dateTimeParsed), remindContent);

        [Command]
        [Summary("Set a reminder for another user")]
        [Priority(2)]
        [RequireModerator]
        public async Task<RuntimeResult> RemindOtherUser(
            [Summary("User")] SocketUser user,
            [Summary("Time")] [OverrideTypeReader(typeof(StringTimeSpanTypeReader))] TimeSpan dateTimeParsed,
            [Summary("Content")] [Remainder] string remindContent) =>
            await Remind(user, DateTimeOffset.Now.Add(dateTimeParsed), remindContent);

        [Command]
        [Summary("Set a reminder")]
        [Priority(1)]
        public async Task<RuntimeResult> RemindUser(
            [Summary("Date Time")] DateTimeOffset dateTime,
            [Summary("Content")] [Remainder] string remindContent) =>
            await Remind(Context.User, dateTime, remindContent);

        [Command]
        [Summary("Set a reminder for another user")]
        [Priority(0)]
        [RequireModerator]
        public async Task<RuntimeResult> RemindOtherUser(
            [Summary("User")] SocketUser user,
            [Summary("Date Time")] DateTimeOffset dateTime,
            [Summary("Content")] [Remainder] string remindContent) =>
            await Remind(user, dateTime, remindContent);

        private async Task<CommandRuntimeResult> Remind(SocketUser user, DateTimeOffset dateTime, string remindContent)
        {
            if (DateTimeOffset.Now > dateTime)
                return CommandRuntimeResult.FromError($"{dateTime} has already passed!");
            var userTimeOffset = await GetUserTimeOffset(user);
            await ReminderService.AddReminder(user, Context.Channel, dateTime, remindContent);
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
        private async Task<TimeSpan> GetUserTimeOffset(SocketUser user) => TimeSpan.Zero;
    }
}