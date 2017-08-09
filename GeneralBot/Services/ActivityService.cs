using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Extensions;
using GeneralBot.Models.Database.CoreSettings;

namespace GeneralBot.Services
{
    internal class ActivityService
    {
        private readonly CoreContext _coreContext;

        public ActivityService(DiscordSocketClient client, CoreContext coreContext)
        {
            _coreContext = coreContext;
            client.UserJoined += UserJoinedAnnounceAsync;
            client.UserLeft += UserLeftAnnounceAsync;
            client.UserVoiceStateUpdated += UserVoiceAnnounceAsync;
        }

        private async Task UserVoiceAnnounceAsync(SocketUser user, SocketVoiceState beforeVoiceState,
            SocketVoiceState afterVoiceState)
        {
            if (!(user is SocketGuildUser guildUser)) return;
            var guild = guildUser.Guild;
            var dbEntry = _coreContext.ActivityLogging.SingleOrDefault(x => x.GuildId == guild.Id);
            if (dbEntry == null || !dbEntry.ShouldLogVoice) return;
            var logChannel = guild.GetTextChannel(dbEntry.LogChannel);
            if (logChannel == null) return;
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = user.GetAvatarUrlOrDefault(),
                    Name = "Voice Event"
                }
            };
            if (afterVoiceState.VoiceChannel != null && beforeVoiceState.VoiceChannel == null)
            {
                embed.Color = Color.Blue;
                embed.Description = $"`{user}` joined {afterVoiceState.VoiceChannel?.Name}!";
                await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
                return;
            }
            if (beforeVoiceState.VoiceChannel != null && afterVoiceState.VoiceChannel == null)
            {
                embed.Color = Color.Orange;
                embed.Description = $"`{user}` left {beforeVoiceState.VoiceChannel?.Name}!";
                await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
                return;
            }
            if (afterVoiceState.VoiceChannel != null && beforeVoiceState.VoiceChannel != null)
            {
                embed.Color = Color.DarkerGrey;
                embed.Description =
                    $"`{user}` moved from {beforeVoiceState.VoiceChannel?.Name} to {afterVoiceState.VoiceChannel?.Name}!";
                await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
            }
        }

        private async Task UserLeftAnnounceAsync(SocketGuildUser guildUser)
        {
            var guild = guildUser.Guild;
            var dbEntry = _coreContext.ActivityLogging.SingleOrDefault(x => x.GuildId == guild.Id);
            if (dbEntry == null || !dbEntry.ShouldLogLeave) return;
            var logChannel = guild.GetTextChannel(dbEntry.LogChannel);
            if (logChannel == null) return;
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = guildUser.GetAvatarUrlOrDefault(),
                    Name = "User Joined"
                },
                Color = Color.Red,
                Description = $"`{guildUser}` ({guildUser.Id}) left the server."
            };
            await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }

        private async Task UserJoinedAnnounceAsync(SocketGuildUser guildUser)
        {
            var guild = guildUser.Guild;
            var dbEntry = _coreContext.ActivityLogging.SingleOrDefault(x => x.GuildId == guild.Id);
            if (dbEntry == null || !dbEntry.ShouldLogJoin) return;
            var logChannel = guild.GetTextChannel(dbEntry.LogChannel);
            if (logChannel == null) return;
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = guildUser.GetAvatarUrlOrDefault(),
                    Name = "User Joined"
                },
                Color = Color.Green,
                Description = $"`{guildUser}` ({guildUser.Id}) joined the server."
            };
            await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }
    }
}