using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerUnitEnteredCombat)]
    class ServerUnitEnteredCombat : IWritable
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
