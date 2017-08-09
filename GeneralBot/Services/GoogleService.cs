using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using GeneralBot.Extensions.Helpers;
using HtmlAgilityPack;
using Humanizer;

namespace GeneralBot.Services
{
    public class GoogleService
    {
        private readonly HttpClient _httpClient;

        public GoogleService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<Embed> SearchAsync(string query, int amount = 5)
        {
            string encodedQuery = WebUtility.UrlEncode(query);
            var builder = new UriBuilder
            {
                Host = "www.google.com",
                Path = "search",
                Query = $"q={encodedQuery}&lr=lang_en&hl=en"
            };

            using (var response = await _httpClient.GetAsync(builder.Uri).ConfigureAwait(false))
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 6.3; Win64; x64)");
                if (response.IsSuccessStatusCode)
                {
                    var embed = new EmbedBuilder();
                    embed.WithColor(ColorHelper.GetRandomColor());
                    string htmlString = await response.Content.ReadAsStringAsync();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlString);
                    var card = await ParseGoogleCardAsync(doc, embed);
                    if (card != null)
                        return card;
                    var results = await GetGoogleSearchAsync(doc, amount);
                    foreach (var result in results)
                        embed.AddField(result.Title, result.Url);
                    return embed.WithFooter(x => x.Text = $"Results for '{query}'").Build();
                }
            }
            return null;
        }

        public Task<List<(string Title, string Url)>> GetGoogleSearchAsync(HtmlDocument doc, int amount = 5)
        {
            var list = new List<(string Title, string Url)>();
            var searchNodes = doc.DocumentNode.SelectNodes("//html//body//div[@class='g']");
            foreach (var node in searchNodes)
            {
                if (list.Count >= amount) break;
                var urlNode = node.SelectSingleNode(".//h3/a");
                if (string.IsNullOrWhiteSpace(urlNode?.InnerText)) continue;
                string parsedTitle = WebUtility.HtmlDecode(urlNode.InnerText);
                string url =
                    WebUtility.UrlDecode(WebUtility.HtmlDecode(urlNode.Attributes.FirstOrDefault(x => x.Name == "href")
                        .Value));
                if (url.StartsWith("/url?q="))
                {
                    url = url.Replace("/url?q=", "");
                    url = url.Remove(url.IndexOf("&sa", StringComparison.OrdinalIgnoreCase));
                    if (list.All(x => x.Url != $"<{url}>"))
                        list.Add((parsedTitle, $"<{url}>"));
                }
            }
            return Task.FromResult(list);
        }

        public async Task<Embed> ParseGoogleCardAsync(HtmlDocument doc, EmbedBuilder builder)
        {
            var featuredSnippet = doc.DocumentNode
                .SelectSingleNode(".//ol/div[@class='g']/div[@class='_uXc hp-xpdbox']")?.ChildNodes;
            if (featuredSnippet != null)
            {
                string icon = featuredSnippet[0]?.SelectSingleNode("div/a/img")?.GetAttributeValue("src", "");
                if (icon != null)
                    builder.ThumbnailUrl = icon;
                if (featuredSnippet.Count > 4)
                {
                    string description = WebUtility.HtmlDecode(featuredSnippet[2].InnerText).Replace("Wikipedia", "");
                    builder.Title =
                        $"{featuredSnippet[1].FirstChild.ChildNodes[0].InnerText} ({featuredSnippet[1].FirstChild.ChildNodes[1].InnerText})";
                    builder.Description = description;
                    var detailedColumn = featuredSnippet.ElementAtOrDefault(4);
                    var nutrition = detailedColumn?.SelectSingleNode("div[@class='_d6d']");
                    if (nutrition != null)
                    {
                        var table = detailedColumn.SelectSingleNode("div/table")?.FirstChild;
                        var numRegex = new Regex(@"(\d.*)");
                        var amount = numRegex.Split(table?.InnerText.Humanize());
                        var calories = numRegex.Split(table?.NextSibling.InnerText);
                        builder.AddInlineField(amount[0], amount[1]);
                        builder.AddInlineField(calories[0], calories[1]);
                    }
                    var googleSearch = await GetGoogleSearchAsync(doc, 3);
                    foreach (var result in googleSearch)
                        builder.AddField(result.Title, result.Url);
                    return builder.Build();
                }
                string unparsedTitle = WebUtility.HtmlDecode(featuredSnippet
                    .FirstOrDefault(x => x.GetAttributeValue("class", "") == "_dSg")?.InnerText);
                if (unparsedTitle != null)
                {
                    var regex = Regex.Split(unparsedTitle, @"((http|https)://\S+\b)");
                    string title;
                    if (regex.Length > 1)
                    {
                        title = regex[0];
                        string url = regex[1];
                        builder.Url = url;
                    }
                    else
                        title = unparsedTitle.Split('-')[0];
                    builder.Title = title;
                    builder.Description = WebUtility.HtmlDecode(featuredSnippet
                        .FirstOrDefault(x => x.GetAttributeValue("class", "") == "_o0d").InnerText);
                    var results = await GetGoogleSearchAsync(doc, 4);
                    foreach (var result in results.Skip(1))
                        builder.AddField(result.Title, result.Url);
                    return builder.Build();
                }
            }

            var words = doc.DocumentNode.SelectSingleNode(".//ol/div[@class='g']/div/h3[@class='r']/div")?.ChildNodes;
            if (words != null)
            {
                var definition = doc.DocumentNode.SelectSingleNode(".//ol/div[@class='g']/div/h3[@class='r']/div");
                var defInfo = definition.ParentNode.ParentNode.ChildNodes[1];
                builder.Title = words[0].InnerText;
                builder.Description = words.Count > 1 ? WebUtility.HtmlDecode(words[1].InnerText) : "";
                foreach (var word in defInfo.ChildNodes)
                {
                    if (word.FirstChild.ChildNodes.FirstOrDefault() == null) continue;
                    string type = word.FirstChild.ChildNodes[0].InnerText;
                    var wordDefinitions = new List<string>();
                    foreach (var def in word.FirstChild.ChildNodes[1].ChildNodes)
                    {
                        if (wordDefinitions.Count == 3)
                            continue;
                        wordDefinitions.Add($"{wordDefinitions.Count + 1}. {WebUtility.HtmlDecode(def.InnerText)}");
                    }
                    builder.AddField(f =>
                    {
                        f.Name = type;
                        f.Value = string.Join("\n", wordDefinitions);
                    });
                }
                return builder.WithFooter(x => x.Text = "Definition").Build();
            }
            var calc = doc.DocumentNode.SelectSingleNode(".//table/tr/td/span[@class='nobr']/h2[@class='r']");
            if (calc != null)
            {
                builder.Description = calc.InnerText;
                return builder.WithFooter(x => x.Text = "Calculator").Build();
            }

            var unitConversion = doc.DocumentNode.SelectSingleNode(".//ol//div[@class='_Tsb']");
            if (unitConversion != null)
            {
                builder.Description = unitConversion.InnerText;
                return builder.WithFooter(x => x.Text = "Unit Conversion").Build();
            }

            var currencyConversion = doc.DocumentNode.SelectSingleNode(".//ol/table[@class='std _tLi']/tr/td/h2");
            if (currencyConversion != null)
            {
                builder.Description = currencyConversion.InnerText;
                return builder.WithFooter(x => x.Text = "Currency Conversion").Build();
            }

            var timeIn = doc.DocumentNode.SelectSingleNode(".//ol//div[@class='_Tsb _HOb _Qeb']");
            if (timeIn != null)
            {
                string timePlace = timeIn.SelectSingleNode("span[@class='_HOb _Qeb']").InnerText;
                string currentTime = timeIn.SelectSingleNode("div[@class='_rkc _Peb']").InnerText;
                string currentDate = timeIn.SelectSingleNode("div[@class='_HOb _Qeb']").InnerText;
                builder.Title = timePlace;
                builder.Description = $"{currentTime} - {currentDate}";
                return builder.WithFooter(x => x.Text = "Time Conversion").Build();
            }

            return null;
        }
    }
}