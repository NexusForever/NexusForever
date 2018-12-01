using System.Text;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public interface ICommandHandler
    {
        int Order { get; }
        bool Handles(CommandContext session, string input);
        void Handle(CommandContext session, string text);
    }
}
