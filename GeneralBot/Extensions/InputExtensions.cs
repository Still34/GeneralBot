using System;
using Discord;
using GeneralBot.Extensions.Helpers;

namespace GeneralBot.Extensions
{
    public static class InputExtensions
    {
        public static bool ContainsCaseInsensitive(this string originalString, string targetString) => originalString.IndexOf(targetString, StringComparison.CurrentCultureIgnoreCase) != -1;

        public static float GetLuminanceFromColor(this Color color) => ColorHelper.GetLuminanceFromColor(color.R, color.G, color.B);
    }
}