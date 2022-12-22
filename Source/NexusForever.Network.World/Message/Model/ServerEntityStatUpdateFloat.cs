using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityStatUpdateFloat)]
    public class ServerEntityStatUpdateFloat : IWritable
    {
        public uint UnitId { get; set; }
        public StatValueUpdate Stat { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            Stat.Write(writer);
        }
    }
}
