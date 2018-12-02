using System.Collections.Generic;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    public interface ICommandHandler
    {
        IEnumerable<string> GetCommands();
        int Order { get; }
        bool Handles(CommandContext session, string input);
        void Handle(CommandContext session, string text);
    }
}
