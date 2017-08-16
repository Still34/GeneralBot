using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Extensions;
using GeneralBot.Models.Config;
using GeneralBot.Models.Database.UserSettings;

namespace DirectoryMaid.Services
{
    public class ReminderService
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigModel _config;
        private readonly IUserRepository _userRepository;
        private Timer _timer;

        public ReminderService()
        {
        }

        public ReminderService(DiscordSocketClient client, IUserRepository userContext, ConfigModel config)
        {
            _client = client;
            _userRepository = userContext;
            _config = config;
            _timer = new Timer(ReminderCheck, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        ///     Gets the <see cref="EmbedBuilder" /> with a consistent reminder style.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public EmbedBuilder GetReminderEmbed(string content) => new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                IconUrl = _client.CurrentUser.GetAvatarUrlOrDefault(),
                Name = "Don't forget to..."
            },
            Description = content,
            Color = Color.Blue,
            ThumbnailUrl = _config.Icons.Calendar
        }.WithCurrentTimestamp();

        /// <summary>
        ///     Creates a new reminder for the specified user.
        /// </summary>
        /// <param name="user">The user which will be reminded.</param>
        /// <param name="channel">The channel for bot to send the reminder to.</param>
        /// <param name="dateTime">When should the user be reminded?</param>
        /// <param name="content">What to remind the user?</param>
        public Task AddReminderAsync(IUser user, IMessageChannel channel, DateTimeOffset dateTime,
            string content) => _userRepository.AddReminderAsync(new Reminder
        {
            ChannelId = channel.Id,
            Content = content,
            Time = dateTime,
            UserId = user.Id
        });

        private void ReminderCheck(object state)
        {
            var remindEntries = _userRepository.GetAllReminders().Where(x => DateTimeOffset.Now > x.Time).ToList();
            if (!remindEntries.Any()) return;
            foreach (var entry in remindEntries)
            {
                RemindUserAsync(entry).ConfigureAwait(false);
                _userRepository.RemoveReminderAsync(entry).ConfigureAwait(false);
            }
        }

        private async Task RemindUserAsync(Reminder reminder)
        {
            var channel = _client.GetChannel(reminder.ChannelId);
            var user = _client.GetUser(reminder.UserId);
            if (channel is IMessageChannel msgChannel && user != null)
                await msgChannel.SendMessageAsync(user.Mention, embed: GetReminderEmbed(reminder.Content))
                    .ConfigureAwait(false);
        }
    }
}