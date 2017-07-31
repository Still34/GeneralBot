using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using GeneralBot.Models.Context;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralBot.Preconditions
{
    public class RequireModerator : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;
            if (guildUser == null) return Task.FromResult(PreconditionResult.FromError("This command must be used in a guild."));
            var settings = services.GetRequiredService<CoreContext>();
            var dbEntry = settings.GuildsSettings.SingleOrDefault(x => x.GuildId == context.Guild.Id);
            // This should never happen
            if (dbEntry == null) return Task.FromResult(PreconditionResult.FromError("This guild has not set up their moderator permission yet."));
            return guildUser.GuildPermissions.Has(dbEntry.ModeratorPermission) ? Task.FromResult(PreconditionResult.FromSuccess()) : Task.FromResult(PreconditionResult.FromError($"Insufficient permission. Required permission: {dbEntry.ModeratorPermission.Humanize()}."));
        }
    }
}