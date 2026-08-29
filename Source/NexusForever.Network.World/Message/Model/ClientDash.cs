using NexusForever.Game.Static.Entity.Movement;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientDash)]
    public class ClientDash : IReadable
    {
        public DashDirection DashDirection { get; set; }

        public void Read(GamePacketReader reader)
        {
            DashDirection = reader.ReadEnum<DashDirection>(3);
        }
    }
}
