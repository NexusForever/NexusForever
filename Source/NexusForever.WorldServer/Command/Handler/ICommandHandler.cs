using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    public interface ICommandHandler
    {
        int Order { get; }
        IEnumerable<string> GetCommands();
        Task<bool> HandlesAsync(CommandContext session, string input, IEnumerable<ChatFormat> chatLinks);
        Task HandleAsync(CommandContext session, string text, IEnumerable<ChatFormat> chatLinks);
    }
}
