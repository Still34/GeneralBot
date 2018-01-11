using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Models.Config;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralBot.Preconditions
{
    /// <summary>
    /// Requires the owners, specified under config.json, to execute.
    /// </summary>
    public class RequireOwners : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var config = services.GetService<ConfigModel>();
            if (config == null)
                return Task.FromResult(
                    PreconditionResult.FromError("This command requires owners to be set in the config first."));
            return config.Owners.Contains(context.User.Id)
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError("Command can only be run by the bot owners."));
        }
    }
}