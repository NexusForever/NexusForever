using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitEnteredCombat)]
    public class ServerUnitEnteredCombat : IWritable
    {
        public uint UnitId { get; set; }
        public bool InCombat { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(InCombat);
        }
    }
}
