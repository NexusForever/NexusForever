using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerTitlesUpdate, MessageDirection.Server)]
    class ServerTitlesUpdate : IWritable
    {
        public List<ulong> Titles { get; set; } = new List<ulong>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Titles.Count);
            foreach (var title in Titles)
            {
                writer.Write(title);
                writer.Write(0, 16); // TODO
            }
        }
    }
}
