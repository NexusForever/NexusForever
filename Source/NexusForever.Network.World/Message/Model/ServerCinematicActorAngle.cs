using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
