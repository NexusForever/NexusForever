using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Auth.Model;

namespace NexusForever.Shared.Game
{
    public class ServerMessageInfo
    {
        public byte Index { get; }
        public List<string> Messages { get; }

        public ServerMessageInfo(IGrouping<byte, ServerMessageModel> group)
        {
            Index    = group.Key;
            Messages = Enumerable.Repeat("", group.Max(s => s.Language) + 1).ToList();

            foreach (ServerMessageModel message in group)
                Messages[message.Language] = message.Message;
        }
    }
}
