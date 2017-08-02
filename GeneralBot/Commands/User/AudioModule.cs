using Discord.Commands;

namespace GeneralBot.Commands.User
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _service;

        public AudioModule(AudioService service) => _service = service;
    }
}