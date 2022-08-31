using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicActorVisibility)]
    public class ServerCinematicActorVisibility : IWritable
    {
        public uint Delay { get; set; }
        public uint UnitId { get; set; }
        public bool Hide { get; set; }
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(UnitId);
            writer.Write(Hide);
            writer.Write(Unknown0);
        }
    }
}
