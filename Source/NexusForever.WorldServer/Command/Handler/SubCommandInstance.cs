using Microsoft.Extensions.Configuration;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Game.Account.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static NexusForever.WorldServer.Command.Handler.CommandCategory;

namespace NexusForever.WorldServer.Command.Handler
{
    public class SubCommandInstance
    {
        public int MinimumStatus { get; }
        public SubCommandHandler Handler { get; }

        public SubCommandInstance(string command, string subCommand, SubCommandHandler handler)
        {
            Handler = handler;

            IEnumerable<KeyValuePair<string, string>> commandData = ConfigurationManager<WorldServerConfiguration>.GetConfiguration().GetSection("Commands").AsEnumerable();
            foreach (var section in commandData)
            {
                var sectionKey = section.Key.Replace(" ", string.Empty).Split(":");

                // The below IF statement checks if this Key is for this NamedCommand
                if (sectionKey.Contains(command) && 
                    sectionKey.Contains(subCommand) &&
                    sectionKey.Contains("SubCommands") &&
                    sectionKey.Contains("MinimumStatus") &&
                    sectionKey.Length <= 8)
                    MinimumStatus = int.Parse(section.Value);
            }
        }
    }
}
