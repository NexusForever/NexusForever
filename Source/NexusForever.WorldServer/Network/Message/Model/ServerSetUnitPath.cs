using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
