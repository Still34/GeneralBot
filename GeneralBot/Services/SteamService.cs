using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneralBot.Models.Config;
using SteamWebAPI2;
using SteamWebAPI2.Interfaces;
using Steam.Models.SteamCommunity;

namespace GeneralBot.Services
{
    public class SteamService
    {
        private readonly SteamUser _steam;

        public SteamService(ConfigModel config)
        {
            _steam = new SteamUser(config.Credentials.Steam);
        }

        public async Task<ulong> GetIdFromvanity(string vanityUrl)
            => (await _steam.ResolveVanityUrlAsync(vanityUrl)).Data;

        public async Task<SteamCommunityProfileModel> GetProfile(ulong id)
            => await _steam.GetCommunityProfileAsync(id);
    }
}
