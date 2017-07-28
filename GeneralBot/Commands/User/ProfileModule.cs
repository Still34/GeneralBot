using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Models;
using GeneralBot.Models.Context;
using GeneralBot.Results;

namespace GeneralBot.Commands.User
{
    [Group("profile")]
    public class ProfileModule : ModuleBase<SocketCommandContext>
    {
        public UserContext UserSettings { get; set; }
        public ConfigModel Config { get; set; }

        [Command("balance")]
        [Summary("Shows your current balance.")]
        public Task<RuntimeResult> Balance()
        {
            var dbEntry = UserSettings.Profile.SingleOrDefault(x => x.UserId == Context.User.Id) ?? UserSettings.Profile.Add(new Profile {UserId = Context.User.Id, LastMessage = Context.Message.Timestamp}).Entity;
            return Task.FromResult<RuntimeResult>(CommandRuntimeResult.FromSuccess($"Your current balance is {dbEntry.Balance}{Config.CurrencySymbol}"));
        }
    }
}