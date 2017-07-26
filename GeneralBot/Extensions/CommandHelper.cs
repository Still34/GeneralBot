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
        public static string GetParamUsage(this ParameterInfo param) => $"{(param.IsOptional ? '[' : '<')}{param.Summary ?? param.Name}{(param.IsOptional ? ']' : '>')}";
    }
}