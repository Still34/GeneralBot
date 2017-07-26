using System.Collections.Generic;
using System.Threading.Tasks;
using GeneralBot.Models;
using Geocoding;
using Geocoding.Google;

namespace GeneralBot.Services
{
    public class GoogleGeocodingService : IGeocoder
    {
        public GoogleGeocoder Geocoder;

        public GoogleGeocodingService(ConfigModel config) => Geocoder = new GoogleGeocoder(config.Credentials.Google);

        public async Task<IEnumerable<Address>> GeocodeAsync(string address) => await Geocoder.GeocodeAsync(address);

        public async Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country) => await Geocoder.GeocodeAsync(BuildAddress(street, city, state, postalCode, country));

        public async Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location) => await Geocoder.ReverseGeocodeAsync(location);

        public async Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude) => await Geocoder.ReverseGeocodeAsync(latitude, longitude);

        private static string BuildAddress(string street, string city, string state, string postalCode, string country) => $"{street} {city}, {state} {postalCode}, {country}";
    }
}