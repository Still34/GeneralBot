using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace GeneralBot.Extensions
{
    public static class GuildExtensions
    {
        /// <summary>
        ///     Gets the last created invite of the channel, null if none (unless <see cref="createNew" /> is specified).
        /// </summary>
        /// <param name="channel">The channel to check the invites in.</param>
        /// <param name="createNew">Whether to create a new invite when none is found.</param>
        /// <returns></returns>
        public static async Task<IInvite> GetLastInviteAsync(this IGuildChannel channel, bool createNew = false)
        {
            var invites = await channel.GetInvitesAsync().ConfigureAwait(false);
            if (invites.Count != 0 || !createNew) return invites.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            return await channel.CreateInviteAsync(null).ConfigureAwait(false);
        }

        /// <summary>
        ///     Gets the last created invite of the channel, null if none (unless <see cref="createNew" /> is specified).
        /// </summary>
        /// <param name="guild">The guild to check for invites.</param>
        /// <param name="createNew">Whether to create a new invite when none is found.</param>
        /// <returns></returns>
        public static async Task<IInvite> GetLastInviteAsync(this IGuild guild, bool createNew = false)
        {
            var invites = await guild.GetInvitesAsync().ConfigureAwait(false);
            if (invites.Count != 0 || !createNew) return invites.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            var defaultChannel = await guild.GetDefaultChannelAsync().ConfigureAwait(false);
            return await defaultChannel.CreateInviteAsync(null).ConfigureAwait(false);
        }

        /// <summary>
        ///     Clones the selected channel.
        /// </summary>
        /// <param name="oldChannel">The <see cref="ITextChannel" /> to be cloned.</param>
        /// <param name="deleteOldChannel">
        ///     Choose whether to delete the old channel or not. If true, the old channel will be
        ///     removed.
        /// </param>
        /// <param name="clonePerms">
        ///     Choose whether to clone the channel permissions or not. If true, the new channel will preserve
        ///     its old counterpart's permission.
        /// </param>
        /// <returns>Return the new <see cref="ITextChannel" />.</returns>
        public static async ValueTask<ITextChannel> CloneChannel(this ITextChannel oldChannel,
            bool deleteOldChannel = false, bool clonePerms = true)
        {
            string channelName = oldChannel.Name;
            string channelTopic = oldChannel.Topic ?? "";
            int channelPosition = oldChannel.Position;
            var channelPerms = oldChannel.PermissionOverwrites;
            var newChannel = await oldChannel.Guild.CreateTextChannelAsync(channelName).ConfigureAwait(false);
            if (clonePerms)
            {
                foreach (var perm in channelPerms)
                {
                    switch (perm.TargetType)
                    {
                        case PermissionTarget.Role:
                            var role = oldChannel.Guild.GetRole(perm.TargetId);
                            await newChannel.AddPermissionOverwriteAsync(role, perm.Permissions).ConfigureAwait(false);
                            break;
                        case PermissionTarget.User:
                            var user = await oldChannel.Guild.GetUserAsync(perm.TargetId).ConfigureAwait(false);
                            await newChannel.AddPermissionOverwriteAsync(user, perm.Permissions).ConfigureAwait(false);
                            break;
                    }
                }
            }
            await newChannel.ModifyAsync(x =>
            {
                x.Position = channelPosition;
                x.Name = channelName;
                x.Topic = channelTopic;
            }).ConfigureAwait(false);
            if (deleteOldChannel)
                await oldChannel.DeleteAsync().ConfigureAwait(false);
            return newChannel;
        }
    }
}