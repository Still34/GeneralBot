using Discord.Commands;

namespace GeneralBot.Commands.User
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        public AudioService AudioService { get; set; }
    }
}