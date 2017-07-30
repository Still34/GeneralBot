using System;
using System.Reflection;
using Discord;
using System.Collections.Generic;

namespace GeneralBot.Extensions.Helpers
{
    public class ColorHelper
    {
        private static List<Color> _colorCache = new List<Color>();

        public static Color GetRandomColor()
        {
            var random = new Random();
            if (_colorCache.Count == 0)
            {
                var fields = typeof(Color).GetFields();
                foreach (var field in fields)
                {
                    var color = field.GetValue(null);
                    if (color is Color)
                        _colorCache.Add((Color)color);
                }
            }
            return _colorCache[random.Next(0, _colorCache.Count)];
        }

        public static float GetLuminanceFromColor(byte r, byte g, byte b) => 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }
}
