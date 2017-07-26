using Discord;

namespace GeneralBot.Templates
{
    public static class EmbedTemplates
    {
        public static EmbedBuilder FromInfo(string description, string title = "") => new EmbedBuilder
        {
            Color = Color.Blue,
            Title = title,
            Description = description
        };
        public static EmbedBuilder FromError(string description, string title = "") => new EmbedBuilder
        {
            Color = Color.Red,
            Title = title,
            Description = description
        };
        public static EmbedBuilder FromSuccess(string description, string title = "") => new EmbedBuilder
        {
            Color = Color.Green,
            Title = title,
            Description = description
        };
        public static EmbedBuilder FromWarning(string description, string title = "") => new EmbedBuilder
        {
            Color = Color.Orange,
            Title = title,
            Description = description
        };
    }
}