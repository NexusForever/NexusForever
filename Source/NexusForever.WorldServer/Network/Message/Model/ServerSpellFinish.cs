using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellFinish, MessageDirection.Server)]

    public class ServerSpellFinish : IWritable
    {
        public uint ServerUniqueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
        }
    }
}