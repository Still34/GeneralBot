using System.Collections.Generic;
using System.Text;
using Discord.Commands;

namespace GeneralBot.Extensions
{
    public static class CommandExtensions
    {
        /// <summary>
        ///     Returns the parameter name with required or optional tag.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetParamUsage(this ParameterInfo param) => $"{(param.IsOptional ? '[' : '<')}{param.Summary ?? param.Name}{(param.IsOptional ? ']' : '>')}";

        public static string GetParamsUsage(this IEnumerable<ParameterInfo> paramInfos)
        {
            var sb = new StringBuilder();
            foreach (var parameterInfo in paramInfos)
                sb.Append($"{GetParamUsage(parameterInfo)} ");
            return sb.ToString();
        }
    }
}