using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Models.Context;

namespace GeneralBot.Services
{
    internal class BalanceService
    {
        private readonly LoggingService _loggingService;
        private readonly UserContext _userSettings;

        public BalanceService(DiscordSocketClient client, UserContext usersettings, LoggingService loggingService)
        {
            client.MessageReceived += AwardBalance;
            _userSettings = usersettings;
            _loggingService = loggingService;
        }

        private async Task AwardBalance(SocketMessage msgArg)
        {
            if (msgArg is SocketUserMessage msg && msg.Channel is SocketGuildChannel && !msg.Author.IsBot)
            {
                var user = msg.Author;
                var dbEntry = _userSettings.Profile.SingleOrDefault(x => x.UserId == user.Id) ?? _userSettings.Profile.Add(new Profile {UserId = user.Id, LastMessage = msg.Timestamp}).Entity;
                uint balanceIncrement = Convert.ToUInt32(new Random().Next(1, 10));
                if (msg.Timestamp >= dbEntry.LastMessage.AddMinutes(1))
                {
                    await _loggingService.Log($"Increasing {user}'s balance by {balanceIncrement}...", LogSeverity.Debug).ConfigureAwait(false);
                    dbEntry.LastMessage = msg.Timestamp;
                    dbEntry.Balance = dbEntry.Balance + balanceIncrement;
                }
                _userSettings.Update(dbEntry);
                await _userSettings.SaveChangesAsync();
            }
        }
    }
}