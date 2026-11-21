using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicActorVisibility)]
    public class ServerCinematicActorVisibility : IWritable
    {
        public uint Delay { get; set; }
        public uint UnitId { get; set; } // affects all units if UnitId is 0
        public bool Hide { get; set; }
        public bool AffectOnlyPlayers { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(UnitId);
            writer.Write(Hide);
            writer.Write(AffectOnlyPlayers);
        }
    }
}
