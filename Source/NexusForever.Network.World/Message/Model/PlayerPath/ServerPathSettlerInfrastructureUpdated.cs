using NexusForever.Game.Static.PlayerPath;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathSettlerInfrastructureUpdated)]
    public class ServerPathSettlerInfrastructureUpdated : IWritable
    {
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
