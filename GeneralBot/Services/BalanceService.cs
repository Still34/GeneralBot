using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Databases.Context;
using GeneralBot.Extensions;
using GeneralBot.Models.Context;

namespace GeneralBot.Services
{
    internal class BalanceService
    {
        private readonly UserContext _userSettings;
        private readonly LoggingService _loggingService;

        public BalanceService(DiscordSocketClient client, UserContext usersettings, LoggingService loggingService)
        {
            client.MessageReceived += AwardBalance;
            _userSettings = usersettings;
            _loggingService = loggingService;
        }

        private async Task AwardBalance(SocketMessage msg)
        {
            var user = msg.Author;
            var dbEntry = _userSettings.Profile.SingleOrDefault(x => x.UserId == user.Id) ?? _userSettings.Profile.Add(new Profile { UserId = user.Id, LastMessage = msg.Timestamp }).Entity;
            var balanceIncrement = Convert.ToUInt32(new Random().Next(1, 10));
            if(dbEntry.LastMessage == null) // Does this ever happen?
            {
                dbEntry.LastMessage = msg.Timestamp;
                dbEntry.Balance = dbEntry.Balance + balanceIncrement;
            }
            else if(msg.Timestamp >= dbEntry.LastMessage.AddMinutes(1))
            {
                dbEntry.LastMessage = msg.Timestamp;
                dbEntry.Balance = dbEntry.Balance + balanceIncrement;
            }
            _userSettings.Update(dbEntry);
            await _userSettings.SaveChangesAsync();
        }
    }
}