using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerInfrastructureUpdated)]
    public class ServerPathSettlerInfrastructureUpdated : IWritable
    {
        public enum SettlerInfrastructureState
        {
            Inactive = 0x0,
            Building = 0x1,
            Built = 0x2,
        };

        public uint PathMissionId { get; set; }
        public SettlerInfrastructureState State { get; set; }
        public float PercentComplete { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathMissionId);
            writer.Write(State, 2);
            writer.Write(PercentComplete);
        }
    }
}
