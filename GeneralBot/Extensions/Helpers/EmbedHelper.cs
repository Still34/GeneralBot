using Discord;

namespace GeneralBot.Extensions.Helpers
{
    public static class EmbedHelper
    {
        public static EmbedBuilder FromInfo(string title = null, string description = null) => new EmbedBuilder
        {
            Color = Color.Blue,
            Title = title ?? "Information",
            Description = description
        };

        public static EmbedBuilder FromError(string title = null, string description = null) => new EmbedBuilder
        {
            Color = Color.Red,
            Title = title ?? "Something happened...",
            Description = description
        };

        public static EmbedBuilder FromSuccess(string title = null, string description = null) => new EmbedBuilder
        {
            Color = Color.Green,
            Title = title ?? "Success!",
            Description = description
        };

        public static EmbedBuilder FromWarning(string title = null, string description = null) => new EmbedBuilder
        {
            Color = Color.Orange,
            Title = title ?? "Warning",
            Description = description
        };
    }
}