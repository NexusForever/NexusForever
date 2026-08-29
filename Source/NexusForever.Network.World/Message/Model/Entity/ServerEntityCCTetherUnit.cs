using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Entity
{
    // If a Tether CCState is applied to the player, the TetherUnit is the unit from which the tether length is calculated.
    [Message(GameMessageOpcode.ServerEntityCCTetherUnit)]
    public class ServerEntityCCTetherUnit : IWritable
    {
        public uint UnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
        }
    }
}
