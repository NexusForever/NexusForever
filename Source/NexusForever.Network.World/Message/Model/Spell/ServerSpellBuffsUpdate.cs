using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    // Changes the number of stacks of buffs applied to target
    [Message(GameMessageOpcode.ServerSpellBuffsUpdate)]
    public class ServerSpellBuffsUpdate : IWritable
    {
        public class BuffInfo : IWritable
        {
            public uint ServerUniqueId { get; set; }
            public uint TargetUnitId { get; set; }
            public uint BuffCount { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ServerUniqueId);
                writer.Write(TargetUnitId);
                writer.Write(BuffCount);
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
