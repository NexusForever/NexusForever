using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitClusterMembership)]
    public class ServerUnitClusterMembership : IWritable
    {
        public uint UnitId { get; set; }
        public uint TargetClusterId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(TargetClusterId);
        }
    }
}
