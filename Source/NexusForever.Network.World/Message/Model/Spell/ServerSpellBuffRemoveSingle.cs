using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellBuffRemoveSingle)]
    public class ServerSpellBuffRemoveSingle : IWritable
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
