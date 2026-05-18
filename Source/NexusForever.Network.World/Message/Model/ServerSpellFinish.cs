using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellFinish)]

    public class ServerSpellFinish : IWritable
    {
        public uint ServerUniqueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
        }
    }
}
