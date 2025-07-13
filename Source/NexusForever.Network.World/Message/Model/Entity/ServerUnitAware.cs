using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Trigger the awareness sound effect and action of the unit.
    [Message(GameMessageOpcode.ServerUnitAware)]
    public class ServerUnitAware : IWritable
    {
        public uint UnitId { get; set; }
        public uint Unused { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Unused);
        }
    }
}
