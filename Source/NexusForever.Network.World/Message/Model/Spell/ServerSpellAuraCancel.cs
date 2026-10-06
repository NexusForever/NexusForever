using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellAuraCancel)]
    public class ServerSpellAuraCancel : IWritable
    {
        public uint TargetUnitId { get; set; }
        public uint ServerUniqueId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TargetUnitId);
            writer.Write(ServerUniqueId);
        }
    }
}
