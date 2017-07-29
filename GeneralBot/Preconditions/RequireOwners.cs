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
            if (config == null) return Task.FromResult(PreconditionResult.FromError("This command requires owners to be set in the config first."));
            return config.Owners.Contains(context.User.Id) ? Task.FromResult(PreconditionResult.FromSuccess()) : Task.FromResult(PreconditionResult.FromError("Command can only be run by the bot owners."));
        }
    }
}