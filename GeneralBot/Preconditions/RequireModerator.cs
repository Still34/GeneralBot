using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Models.Database.CoreSettings;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralBot.Preconditions
{
    /// <summary>
    ///     Requires the guild-specific moderator permission to execute.
    /// </summary>
    public class RequireModerator : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;
            if (guildUser == null)
                return PreconditionResult.FromError("This command must be used in a guild.");
            var settings = services.GetRequiredService<ICoreRepository>();
            var record = await settings.GetOrCreateGuildSettingsAsync(context.Guild).ConfigureAwait(false);
            return guildUser.GuildPermissions.Has(record.ModeratorPermission)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError(
                    $"Insufficient permission. Required permission: {record.ModeratorPermission.Humanize()}.");
        }
    }
}