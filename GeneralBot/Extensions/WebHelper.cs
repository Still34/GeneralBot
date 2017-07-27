using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GeneralBot.Extensions
{
    public class WebHelper
    {
        public static async Task<string> GetMediaHeader(Uri uri, TimeSpan? timeout = null)
        {
            try
            {
                using (var client = new HttpClient {Timeout = timeout ?? TimeSpan.FromSeconds(5)})
                using (var response = await client.GetAsync(uri))
                {
                    return response.IsSuccessStatusCode ? response.Content.Headers.ContentType.MediaType : null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Stream> GetFile(Uri uri, TimeSpan? timeout = null)
        {
            try
            {
                using (var client = new HttpClient {Timeout = timeout ?? TimeSpan.FromSeconds(5)})
                {
                    var response = await client.GetAsync(uri);
                    return await response.Content.ReadAsStreamAsync();
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Uri> GetImageUri(string input)
        {
            var regex = Regex.Match(input, @"\b\w+://\S+\b", RegexOptions.IgnoreCase);
            if (!regex.Success || !Uri.TryCreate(regex.Value, UriKind.RelativeOrAbsolute, out Uri uri)) return null;
            string header = await GetMediaHeader(uri);
            return header.StartsWith("image") ? uri : null;
        }
    }
}