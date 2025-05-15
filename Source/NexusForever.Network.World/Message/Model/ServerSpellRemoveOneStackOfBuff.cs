using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellRemoveOneStackOfBuff)]
    public class ServerSpellRemoveOneStackOfBuff : IWritable
    {
        public uint ServerUniqueId { get; set; }
        public uint TargetUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(TargetUnitId);
        }
    }
}
