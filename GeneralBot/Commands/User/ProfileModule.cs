using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Models.Context;
using GeneralBot.Results;
using GeneralBot.Services;
using GeneralBot.Templates;
using GeneralBot.Models;

namespace GeneralBot.Commands.User
{
    [Group("profile")]
    public class ProfileModule : ModuleBase<SocketCommandContext>
    {
        public readonly UserContext UserSettings;
        public readonly ConfigModel Config;

        [Command("balance")]
        [Summary("Shows your current balance.")]
        public async Task<RuntimeResult> Balance()
        {
            var dbEntry = UserSettings.Profile.SingleOrDefault(x => x.UserId == Context.User.Id) ?? UserSettings.Profile.Add(new Profile { UserId = Context.User.Id, LastMessage = Context.Message.Timestamp }).Entity;
            return CommandRuntimeResult.FromSuccess($"Your current balance is {dbEntry.Balance}{Config.CurrencySymbol}");
        }
    }
}