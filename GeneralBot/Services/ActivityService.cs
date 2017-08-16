using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GeneralBot.Extensions;
using GeneralBot.Models.Database.CoreSettings;

namespace GeneralBot.Services
{
    public class ActivityService
    {
        private readonly ICoreRepository _coreRepository;

        public ActivityService(DiscordSocketClient client, ICoreRepository coreContext)
        {
            _coreRepository = coreContext;
            client.UserJoined += UserJoinedAnnounceAsync;
            client.UserLeft += UserLeftAnnounceAsync;
            client.UserVoiceStateUpdated += UserVoiceAnnounceAsync;
        }

        private async Task UserVoiceAnnounceAsync(SocketUser user, SocketVoiceState beforeVoiceState,
            SocketVoiceState afterVoiceState)
        {
            if (!(user is SocketGuildUser guildUser)) return;
            var guild = guildUser.Guild;
            var record = await _coreRepository.GetOrCreateActivityAsync(guild).ConfigureAwait(false);
            if (!record.ShouldLogVoice) return;
            var logChannel = guild.GetTextChannel(record.LogChannel);
            if (logChannel == null) return;
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = user.GetAvatarUrlOrDefault()
                }
            };
            if (afterVoiceState.VoiceChannel != null && beforeVoiceState.VoiceChannel == null)
            {
                embed.Color = Color.Blue;
                embed.Title = "Voice Joined";
                embed.Description = $"{user} ({user.Id}) joined {Format.Bold(afterVoiceState.VoiceChannel?.Name)}!";
                await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
                return;
            }
            if (beforeVoiceState.VoiceChannel != null && afterVoiceState.VoiceChannel == null)
            {
                embed.Color = Color.Orange;
                embed.Title = "Voice Left";
                embed.Description = $"{user} ({user.Id}) left {Format.Bold(beforeVoiceState.VoiceChannel?.Name)}!";
                await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
                return;
            }
            if (afterVoiceState.VoiceChannel != null && beforeVoiceState.VoiceChannel != null)
            {
                embed.Color = Color.DarkerGrey;
                embed.Title = "Voice Changed Channel";
                embed.Description =
                    $"{user} ({user.Id}) moved from {Format.Bold(beforeVoiceState.VoiceChannel?.Name)} to {Format.Bold(afterVoiceState.VoiceChannel?.Name)}!";
                await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
            }
        }

        private async Task UserLeftAnnounceAsync(SocketGuildUser guildUser)
        {
            var guild = guildUser.Guild;
            var record = await _coreRepository.GetOrCreateActivityAsync(guild).ConfigureAwait(false);
            if (!record.ShouldLogLeave) return;
            var logChannel = guild.GetTextChannel(record.LogChannel);
            if (logChannel == null) return;
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = guildUser.GetAvatarUrlOrDefault(),
                    Name = "User Joined"
                },
                Color = Color.Red,
                Description = $"{guildUser} ({guildUser.Id}) left the server."
            };
            await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }

        private async Task UserJoinedAnnounceAsync(SocketGuildUser guildUser)
        {
            var guild = guildUser.Guild;
            var record = await _coreRepository.GetOrCreateActivityAsync(guild).ConfigureAwait(false);
            if (!record.ShouldLogJoin) return;
            var logChannel = guild.GetTextChannel(record.LogChannel);
            if (logChannel == null) return;
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = guildUser.GetAvatarUrlOrDefault(),
                    Name = "User Joined"
                },
                Color = Color.Green,
                Description = $"{guildUser} ({guildUser.Id}) joined the server."
            };
            await logChannel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }
    }
}