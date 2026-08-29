using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicActorRemove)]
    public class ServerCinematicActorRemove : IWritable
    {
        public uint Delay { get; set; }
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(UnitId);
        }
    }
}
