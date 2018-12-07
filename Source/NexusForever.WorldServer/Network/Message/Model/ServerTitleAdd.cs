using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerTitleAdd, MessageDirection.Server)]
    class ServerTitleAdd : IWritable
    {
        public ulong Title { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Title, 14);
        }
    }
}
