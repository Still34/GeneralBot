using Discord.Commands;

namespace GeneralBot.Extensions
{
    public static class CommandHelper
    {
        /// <summary>
        ///     Returns the parameter name with required or optional tag.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetParamUsage(this ParameterInfo param)
        {
            char bracketFront = param.IsOptional ? '[' : '<';
            char bracketBehind = param.IsOptional ? ']' : '>';
            return $"{bracketFront}{param.Summary ?? param.Name}{bracketBehind}";
        }
    }
}