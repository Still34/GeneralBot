using System;
using Discord;
using GeneralBot.Extensions.Helpers;

namespace GeneralBot.Extensions
{
    public static class InputExtensions
    {
        public static bool ContainsCaseInsensitive(this string originalString, string targetString) => originalString.IndexOf(targetString, StringComparison.CurrentCultureIgnoreCase) != -1;

        public static float GetLuminanceFromColor(this Color color) => ColorHelper.GetLuminanceFromColor(color.R, color.G, color.B);

        /// <summary>
        ///     Limits the range of the <see cref="int" />.
        /// </summary>
        /// <param name="value">The value to limit for.</param>
        /// <param name="min">The minimum value for the <see cref="int" />.</param>
        /// <param name="max">The maximum value for the <see cref="int" />.</param>
        /// <returns></returns>
        public static int LimitToRange(this int value, int min, int max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
    }
}