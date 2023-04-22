using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSetUnitPathType)]
    public class ServerSetUnitPathType : IWritable
    {
        public uint Guid { get; set; }
        public Path Path { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Path, 3);
        }
    }
}
