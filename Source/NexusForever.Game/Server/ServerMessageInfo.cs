using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Server;

namespace NexusForever.Game.Server
{
    public class ServerMessageInfo : IServerMessageInfo
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
