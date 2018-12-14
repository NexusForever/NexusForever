using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    public interface ICommandHandler
    {
        int Order { get; }
        IEnumerable<string> GetCommands();
        Task<bool> HandlesAsync(CommandContext session, string input);
        Task HandleAsync(CommandContext session, string text);
    }
}
