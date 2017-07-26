using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralBot.Preconditions
{
    public class RequireOwners : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var config = services.GetService<ConfigModel>();
            return config.Owners.Contains(context.User.Id) ? Task.FromResult(PreconditionResult.FromSuccess()) : Task.FromResult(PreconditionResult.FromError("Command can only be run by the bot owners."));
        }
    }
}