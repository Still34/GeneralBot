using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GeneralBot.Typereaders
{
    public class GuildPermissionTypeReader : TypeReader
    {
        public static Type[] Types { get; } = {typeof(GuildPermission)};

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            switch (input.ToLower())
            {
                case var i when i.Contains("create"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.CreateInstantInvite));
                case var i when i.Contains("kick"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.KickMembers));
                case var i when i.Contains("ban"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.BanMembers));
                case var i when i.Contains("admin"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.Administrator));
                case var i when i.Contains("manage"):
                    switch (i)
                    {
                        case var ii when ii.Contains("message"):
                            return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ManageMessages));
                        case var ii when ii.Contains("guild"):
                            return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ManageGuild));
                        case var ii when ii.Contains("channel"):
                            return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ManageChannels));
                        case var ii when ii.Contains("nickname"):
                            return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ManageNicknames));
                        case var ii when ii.Contains("role"):
                            return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ManageRoles));
                        case var ii when ii.Contains("webhook"):
                            return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ManageWebhooks));
                        case var ii when ii.Contains("emoji"):
                            return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ManageEmojis));
                    }
                    break;
                case var i when i.Contains("add") || i.Contains("reaction"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.AddReactions));
                case var i when i.Contains("read"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ViewChannel));
                case var i when i.Contains("history"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ReadMessageHistory));
                case var i when i.Contains("send"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.SendMessages));
                case var i when i.Contains("tts"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.SendTTSMessages));
                case var i when i.Contains("embed"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.EmbedLinks));
                case var i when i.Contains("attach") || i.Contains("upload"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.AttachFiles));
                case var i when i.Contains("mention") || i.Contains("everyone"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.MentionEveryone));
                case var i when i.Contains("external") || i.Contains("emoji"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.UseExternalEmojis));
                case var i when i.Contains("connect"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.Connect));
                case var i when i.Contains("speak"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.Speak));
                case var i when i.Contains("mute"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.MuteMembers));
                case var i when i.Contains("deafen"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.DeafenMembers));
                case var i when i.Contains("move"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.MoveMembers));
                case var i when i.Contains("voice") || i.Contains("activity") || i.Contains("push"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.UseVAD));
                case var i when i.Contains("nickname"):
                    return Task.FromResult(TypeReaderResult.FromSuccess(GuildPermission.ChangeNickname));
            }
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Cannot find the desired permission."));
        }
    }
}