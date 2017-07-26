using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralBot.Extensions
{
    public static class InputExtensions
    {
        public static bool ContainsCaseInsensitive(this string originalString, string targetString)
        {
            return originalString.IndexOf(targetString, StringComparison.CurrentCultureIgnoreCase) != -1;
        }
    }
}
