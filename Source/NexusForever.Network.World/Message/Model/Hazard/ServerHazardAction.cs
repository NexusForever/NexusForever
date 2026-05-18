using NexusForever.Game.Static.Hazard;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Hazard
{
    [Message(GameMessageOpcode.ServerHazardAction)]
    public class ServerHazardAction : IWritable
    {
        public uint HazardId { get; set; }
        public HazardAction Action { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(HazardId, 14u);
            writer.Write(Action, 3u);
        }
    }
}
