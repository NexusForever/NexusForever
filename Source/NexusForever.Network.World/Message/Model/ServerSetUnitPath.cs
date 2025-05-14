using NexusForever.Network.Message;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSetUnitPathType)]
    public class ServerSetUnitPathType : IWritable
    {
        public uint UnitId { get; set; }
        public Path Path { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Path, 3);
        }
    }
}
