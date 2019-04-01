using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityCombatStateUpdate, MessageDirection.Server)]
    class ServerEntityCombatStateUpdate : IWritable
    {
        public uint UnitId { get; set; }
        public bool CombatState { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(CombatState);
        }
    }
}
