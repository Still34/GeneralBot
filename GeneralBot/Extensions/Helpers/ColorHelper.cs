using System;
using System.Reflection;
using Discord;

namespace GeneralBot.Extensions.Helpers
{
    public class ColorHelper
    {
        private List<Color> _colorCache = new List<Color>;

        public static Color GetRandomColor()
        {
            if (_colorCache.Length == 0)
            {
                var random = new Random();
                var fields = typeof(Color).GetFields();
                foreach (var field in fields)
                {
                    var color = field.GetValue(null);
                    if (color is Color)
                        _colorCache.Add(color);
                }
            }
            return _colorCache[random.Next(0, _colorCache.Length)];
        }


        public static float GetLuminanceFromColor(byte r, byte g, byte b) => 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }
}
