using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneralBot.Extensions.Helpers
{
    public class WebHelper
    {
        public static async Task<string> GetMediaHeaderAsync(HttpClient client, Uri uri, TimeSpan? timeout = null)
        {
            try
            {
                using (var response = await client.GetAsync(uri).ConfigureAwait(false))
                {
                    return response.IsSuccessStatusCode ? response.Content.Headers.ContentType.MediaType : null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Stream> GetFileAsync(HttpClient client, Uri uri, TimeSpan? timeout = null)
        {
            try
            {
                var response = await client.GetAsync(uri).ConfigureAwait(false);
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Uri> GetImageUriAsync(HttpClient client, string input)
        {
            var regex = Regex.Match(input, @"\b\w+://\S+\b", RegexOptions.IgnoreCase);
            if (!regex.Success || !Uri.TryCreate(regex.Value, UriKind.RelativeOrAbsolute, out Uri uri)) return null;
            string header = await GetMediaHeaderAsync(client, uri).ConfigureAwait(false);
            return header.StartsWith("image") ? uri : null;
        }
    }
}