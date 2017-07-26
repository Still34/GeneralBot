using System;
using System.Reflection;
using Discord;

namespace GeneralBot.Extensions
{
    public static class InputHelper
    {
        public static bool ContainsCaseInsensitive(this string originalString, string targetString) => originalString.IndexOf(targetString, StringComparison.CurrentCultureIgnoreCase) != -1;

        public static Color GetRandomColor()
        {
            var random = new Random();
            var fields = typeof(Color).GetFields();
            var color = (Color) fields[random.Next(0, fields.Length)].GetValue(null);
            return color;
        }
    }
}