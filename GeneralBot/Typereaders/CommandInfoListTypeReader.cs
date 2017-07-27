using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using GeneralBot.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralBot.Typereaders
{
    public class CommandInfoListTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            var commandService = services.GetRequiredService<CommandService>();
            var commandInfos = new List<CommandInfo>();
            foreach (var module in commandService.Modules)
            {
                foreach (var command in module.Commands)
                {
                    var check = await command.CheckPreconditionsAsync(context, services);
                    if (!check.IsSuccess) continue;
                    if (command.Aliases.Any(x => x.ContainsCaseInsensitive(input)) ||
                        module.IsSubmodule &&
                        module.Aliases.Any(x => x.ContainsCaseInsensitive(input)))
                        commandInfos.Add(command);
                }
            }
            return TypeReaderResult.FromSuccess(commandInfos);
        }
    }
}