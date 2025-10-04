using NexusForever.Game.Static.PublicEvent;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerPublicEventBombStatus)]
    public class ServerPublicEventBombStatus : IWritable
    {
        public PublicEventTeam Team { get; set; }
        public uint CarrierUnitId { get; set; }
        public uint TimeInMs { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CarrierUnitId);
            writer.Write(TimeInMs);
        }
    }
}
