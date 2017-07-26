using Discord;

namespace GeneralBot.Templates
{
    public static class EmbedTemplates
    {
        public static EmbedBuilder FromInfo(string description, string title = null) => new EmbedBuilder
        {
            Color = Color.Blue,
            Title = title ?? "Information",
            Description = description
        };
        public static EmbedBuilder FromError(string description, string title = null) => new EmbedBuilder
        {
            Color = Color.Red,
            Title = title ?? "Something happened...",
            Description = description
        };
        public static EmbedBuilder FromSuccess(string description, string title = null) => new EmbedBuilder
        {
            Color = Color.Green,
            Title = title ?? "Success!",
            Description = description
        };
        public static EmbedBuilder FromWarning(string description, string title = null) => new EmbedBuilder
        {
            Color = Color.Orange,
            Title = title ?? "Warning",
            Description = description
        };
    }
}