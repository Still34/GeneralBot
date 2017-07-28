using System;
using System.Reflection;
using Discord;

namespace GeneralBot.Extensions.Helpers
{
    public class ColorHelper
    {
        public static Color GetRandomColor()
        {
            var random = new Random();
            var fields = typeof(Color).GetFields();
            var color = (Color) fields[random.Next(0, fields.Length)].GetValue(null);
            return color;
        }

        public static float GetLuminanceFromColor(byte r, byte g, byte b) => 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }
}