using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Models.Database.UserSettings;

namespace GeneralBot.Services
{
    public class BalanceService
    {
        private readonly LoggingService _loggingService;
        private readonly IUserRepository _userSettings;
        private readonly Random _random;        

        public BalanceService(DiscordSocketClient client, IUserRepository usersettings, LoggingService loggingService,
            Random random)
        {
            client.MessageReceived += AwardBalanceAsync;
            _userSettings = usersettings;
            _loggingService = loggingService;
            _random = random;
        }

        private async Task AwardBalanceAsync(SocketMessage msgArg)
        {
            if (msgArg is SocketUserMessage msg &&
                msg.Channel is SocketGuildChannel &&
                !msg.Author.IsBot)
            {
                var user = msg.Author;
                var record = await _userSettings.GetOrCreateProfileAsync(user).ConfigureAwait(false);
                uint balanceIncrement = Convert.ToUInt32(_random.Next(1, 10));
                if (msg.Timestamp >= record.LastMessage.AddMinutes(1))
                {
                    await _loggingService.LogAsync($"Increasing {user}'s balance by {balanceIncrement}...",
                        LogSeverity.Debug).ConfigureAwait(false);
                    record.LastMessage = msg.Timestamp;
                    record.Balance = record.Balance + balanceIncrement;
                    await _userSettings.SaveRepositoryAsync().ConfigureAwait(false);
                }
            }
        }

        public int GetBalanceForLevel(int level)
        {
            switch (level)
            {
                case 0:
                    return 0;
                case 1:
                    return 20;
            }
            var lastLevel = GetBalanceForLevel(level - 1);
            return lastLevel + level * 20;
        }

        public int GetLevel(uint balance)
        {
            int level = 0;
            while (balance >= GetBalanceForLevel(level))
                level++;
            return level - 1;
        }

        public int GetRank(uint balance)
        {
            var balances = _userSettings.GetProfiles().Select(x=> x.Balance);
            var rank = balances.OrderByDescending(x=> x).ToList().IndexOf(balance);
            return rank + 1;
        }
    }
}