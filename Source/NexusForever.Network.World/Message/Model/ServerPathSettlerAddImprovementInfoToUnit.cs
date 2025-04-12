using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerAddImprovementInfoToUnit)]
    public class ServerPathSettlerAddImprovementInfoToUnit : IWritable
    {
        public class ImprovementInfo : IWritable
        {
            public string Name { get; set; }
            public uint Tier { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(Name);
                writer.Write(Tier);
            }
        }

        public uint UnitId { get; set; }
        public uint PathSettlerImprovementGroupId { get; set; }
        public uint RemainingTime { get; set; }
        public List<string> Owners { get; set; } = new List<string>();
        public List<ImprovementInfo> Improvements { get; set; } = new List<ImprovementInfo>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(PathSettlerImprovementGroupId, 14);
            writer.Write(RemainingTime);
            writer.Write(Owners.Count);
            foreach (var owner in Owners)
            {
                writer.WriteStringWide(owner);
            
            }
            writer.Write(Improvements.Count);
            foreach (var improvement in Improvements)
            {
                improvement.Write(writer);
            }
        }
    }
}
