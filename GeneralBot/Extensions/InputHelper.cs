using System;

namespace GeneralBot.Extensions
{
    public static class InputHelper
    {
        public static bool ContainsCaseInsensitive(this string originalString, string targetString) => originalString.IndexOf(targetString, StringComparison.CurrentCultureIgnoreCase) != -1;
    }
}