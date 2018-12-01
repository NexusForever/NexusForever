using System.Collections.Generic;
using System.Text;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

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
