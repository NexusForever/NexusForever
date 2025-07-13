using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEmote)]
    public class ServerEmote : IWritable
    {
        public uint EmotesId { get; set; }
        public uint Seed { get; set; }
        public uint SourceUnitId { get; set; }
        public uint TargetUnitId { get; set; } 
        public bool Targeted { get; set; }
        public bool Silent { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(EmotesId);
            writer.Write(Seed);
            writer.Write(SourceUnitId);
            writer.Write(TargetUnitId);
            writer.Write(Targeted);
            writer.Write(Silent);
        }
    }
}
