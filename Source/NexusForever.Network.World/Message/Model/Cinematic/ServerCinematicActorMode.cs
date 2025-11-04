using NexusForever.Game.Static.Entity.Movement.Command.Mode;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicActorMode)]
    public class ServerCinematicActorMode : IWritable
    {
        public uint Delay { get; set; }
        public uint UnitId { get; set; }
        public ModeType Mode { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(UnitId);
            writer.Write(Mode);
        }
    }
}
