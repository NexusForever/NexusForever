using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityEmote)]
    public class ServerEntityEmote : IWritable
    {
        public ushort EmotesId { get; set; } // 14u
        public uint Seed { get; set; }
        public uint SourceUnitId { get; set; }
        public uint TargetUnitId { get; set; }
        public bool Targeted { get; set; }
        public bool Silent { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EmotesId, 14u);
            writer.Write(Seed);
            writer.Write(SourceUnitId);
            writer.Write(TargetUnitId);
            writer.Write(Targeted);
            writer.Write(Silent);
        }
    }
}
