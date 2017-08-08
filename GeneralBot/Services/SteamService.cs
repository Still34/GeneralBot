using System.Threading.Tasks;
using GeneralBot.Models.Config;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;

namespace GeneralBot.Services
{
    public class SteamService
    {
        private readonly SteamUser _steam;

        public SteamService(ConfigModel config) => _steam = new SteamUser(config.Credentials.Steam);

        public async Task<ulong> GetIdFromVanityAsync(string vanityUrl)
            => (await _steam.ResolveVanityUrlAsync(vanityUrl).ConfigureAwait(false)).Data;

        public async Task<SteamCommunityProfileModel> GetProfileAsync(ulong id)
            => await _steam.GetCommunityProfileAsync(id).ConfigureAwait(false);
    }
}