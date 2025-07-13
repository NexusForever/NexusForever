using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Only useful to send if Reason is OutOfRange otherwise client does not trigger the UnitEvaded event
    [Message(GameMessageOpcode.ServerUnitEvaded)]
    public class ServerUnitEvaded : IWritable
    {
        public uint UnitId { get; set; }
        public EvadedReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(Reason, 32u);
        }
    }
}
