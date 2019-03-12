using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeneralBot.Models
{
    public class MediaEmbed
    {
        public string Content { get; set; }
        public int? Height { get; set; }
        public bool? Scrolling { get; set; }
        public int? Width { get; set; }
    }

    public class Embed
    {
        public string Description { get; set; }
        public int Height { get; set; }
        public string Html { get; set; }
        public string Provider { get; set; }
        public string ProviderUrl { get; set; }
        public string Thumbnail { get; set; }
        public int ThumbnailHeight { get; set; }
        public int ThumbnailWidth { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public int Width { get; set; }
    }

    public class SecureMedia
    {
        [JsonProperty("O" + "embed")]
        public Embed Embed { get; set; }
        public string Type { get; set; }
    }

    public class SecureMediaEmbed
    {
        public string Content { get; set; }
        public int? Height { get; set; }
        public bool? Scrolling { get; set; }
        public int? Width { get; set; }
    }

    public class Source
    {
        public int Height { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Resolution
    {
        public int Height { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Gif
    {
        public IList<Resolution> Resolutions { get; set; }
        public Source Source { get; set; }
    }

    public class Mp4
    {
        public IList<Resolution> Resolutions { get; set; }
        public Source Source { get; set; }
    }

    public class Variants
    {
        public Gif Gif { get; set; }
        public Mp4 Mp4 { get; set; }
    }

    public class Image
    {
        public string Id { get; set; }
        public IList<Resolution> Resolutions { get; set; }
        public Source Source { get; set; }
        public Variants Variants { get; set; }
    }

    public class Preview
    {
        public bool Enabled { get; set; }
        public IList<Image> Images { get; set; }
    }

    public class Media
    {
        public Embed Embed { get; set; }
        public string Type { get; set; }
    }

    public class PostData
    {
        public object ApprovedAt { get; set; }
        public object ApprovedBy { get; set; }
        public bool Archived { get; set; }
        public string Author { get; set; }
        public object AuthorFlairCss { get; set; }
        public object AuthorFlairText { get; set; }
        public object BannedAt { get; set; }
        public object BannedBy { get; set; }
        public bool BrandSafe { get; set; }
        public bool CanGild { get; set; }
        public bool CanModPost { get; set; }
        public bool Clicked { get; set; }
        public bool ContestMode { get; set; }
        public double Created { get; set; }
        public double CreatedUtc { get; set; }
        public object Distinguished { get; set; }
        public string Domain { get; set; }
        public int Downs { get; set; }
        public bool Edited { get; set; }
        public int Gilded { get; set; }
        public bool Hidden { get; set; }
        public bool HideScore { get; set; }
        public string Id { get; set; }
        public bool IsNsfw { get; set; }
        public bool IsSelf { get; set; }
        public bool IsVideo { get; set; }
        public object Likes { get; set; }
        public object LinkFlairCss { get; set; }
        public object LinkFlairText { get; set; }
        public bool Locked { get; set; }
        public Media Media { get; set; }
        public MediaEmbed MediaEmbed { get; set; }
        public IList<object> ModReports { get; set; }
        public string Name { get; set; }
        public int NumComments { get; set; }
        public object NumReports { get; set; }
        public string Permalink { get; set; }
        public string PostHint { get; set; }
        public Preview Preview { get; set; }
        public bool Quarantine { get; set; }
        public object RemovalReason { get; set; }
        public object ReportReasons { get; set; }
        public bool Saved { get; set; }
        public int Score { get; set; }
        public SecureMedia SecureMedia { get; set; }
        public SecureMediaEmbed SecureMediaEmbed { get; set; }
        public string SelfText { get; set; }
        public object SelfTextHtml { get; set; }
        public bool Spoiler { get; set; }
        public bool Stickied { get; set; }
        public string Subreddit { get; set; }
        public string SubredditId { get; set; }
        public string SubredditNamePrefixed { get; set; }
        public string SubredditType { get; set; }
        public object SuggestedSort { get; set; }
        public string Thumbnail { get; set; }
        public int ThumbnailHeight { get; set; }
        public int ThumbnailWidth { get; set; }
        public string Title { get; set; }
        public int Ups { get; set; }
        public string Url { get; set; }
        public IList<object> UserReports { get; set; }
        public object ViewCount { get; set; }
        public bool Visited { get; set; }
    }

    public class Child
    {
        public PostData Data { get; set; }
        public string Kind { get; set; }
    }

    public class Data
    {
        public string After { get; set; }
        public object Before { get; set; }
        public IList<Child> Children { get; set; }
        public string Modhash { get; set; }
    }

    public class RedditResponseModel
    {
        public Data Data { get; set; }
        public string Kind { get; set; }
    }
}