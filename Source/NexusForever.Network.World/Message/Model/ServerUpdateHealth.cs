using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUpdateHealth)]
    public class ServerUpdateHealth : IWritable
    {
        public uint UnitId { get; set; }
        public uint Health { get; set; }
        public UpdateHealthMask Mask { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Health);
            writer.Write(Mask, 18u);
        }
    }
}
