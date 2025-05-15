using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Triggers buffs and spelleffects from ActiveSpell on the client to be applied to the target.
    [Message(GameMessageOpcode.ServerSpellBuffsAdd)]
    public class ServerSpellBuffsAdd : IWritable
    {
        public class BuffInfo : IWritable
        { 
            public uint ServerUniqueId { get; set; }
            public uint TargetUnitId { get; set; }
            public uint Unused { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ServerUniqueId);
                writer.Write(TargetUnitId);
                writer.Write(Unused);
            }
        };

        public List<BuffInfo> Buffs { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Buffs.Count);
            foreach (var buff in Buffs)
            {
                buff.Write(writer);
            }
        }
    }
}
