using System;
using System.Collections.Generic;
using System.Reflection;
using Discord;

namespace GeneralBot.Extensions.Helpers
{
    public class ColorHelper
    {
        private static readonly List<Color> ColorCache = new List<Color>();

        public static Color GetRandomColor()
        {
            var random = new Random();
            if (ColorCache.Count == 0)
            {
                var fields = typeof(Color).GetFields();
                foreach (var field in fields)
                    if (field.GetValue(null) is Color color) ColorCache.Add(color);
            }
            return ColorCache[random.Next(0, ColorCache.Count)];
        }

        public static float GetLuminanceFromColor(byte r, byte g, byte b) => 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }
}