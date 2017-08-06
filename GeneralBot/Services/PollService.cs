using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Models.Database.CoreSettings;
using GeneralBot.Models.Database.CoreSettings.Poll;

namespace GeneralBot.Services
{
    public class PollService
    {
        private readonly CoreContext _coreSettings;
        private readonly DiscordSocketClient _discordClient;
        private readonly LoggingService _loggingService;

        public PollService(CoreContext coreSettings, LoggingService loggingService, DiscordSocketClient discordClient)
        {
            _coreSettings = coreSettings;
            _loggingService = loggingService;
            _discordClient = discordClient;
            _discordClient.ReactionAdded += VoteProcessAsync;
            new Timer(PollCheck, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        private void PollCheck(object state)
        {
            var entries =
                _coreSettings.PollsData.Where(x => (DateTimeOffset.UtcNow > x.EndDate) &
                                                   (x.EndDate != DateTimeOffset.MinValue));
            foreach (var entry in entries)
            {
            }
        }

        private async Task HandlePollResultAsync(PollData entryData)
        {
            if (!(_discordClient.GetChannel(entryData.MainChannel) is SocketTextChannel channel)) return;
            var pollMessage = await channel.GetMessageAsync(entryData.MessageId) as SocketUserMessage;
            if (pollMessage == null)
            {
                await _loggingService.LogAsync(
                        $"Message for poll {entryData.MessageId} could not be found, dropping entry.",
                        LogSeverity.Warning)
                    .ConfigureAwait(false);
                await EntryCleanupAsync(entryData);
                return;
            }
            var pollReactions = pollMessage.Reactions;
        }

        private async Task EntryCleanupAsync(PollData entryData)
        {
            entryData.EndDate = DateTimeOffset.MinValue;
            entryData.MessageId = 0;
            _coreSettings.Update(entryData);
            await _coreSettings.SaveChangesAsync();
        }

        private Task VoteProcessAsync(Cacheable<IUserMessage, ulong> msgParam, ISocketMessageChannel channel,
            SocketReaction reaction) => throw new NotImplementedException();
    }
}