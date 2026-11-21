using NexusForever.Game.Static.Cinematic;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicNotify)]
    public class ServerCinematicNotify : IWritable
    {
        public CinematicFlags Flags { get; set; }
        public CancelType CancelMode { get; set; }
        public uint Delay { get; set; } // for scheduling end
        public ushort CinematicId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Flags, 16u);
            writer.Write(CancelMode, 16u);
            writer.Write(Delay);
            writer.Write(CinematicId, 14u);
        }
    }
}
