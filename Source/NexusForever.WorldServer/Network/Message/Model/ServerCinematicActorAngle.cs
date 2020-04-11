using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicActorAngle)]
    public class ServerCinematicActorAngle : IWritable
    {
        public uint Delay { get; set; }
        public uint UnitId { get; set; }
        public float Angle { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(UnitId);
            writer.Write(Angle);
        }
    }
}
