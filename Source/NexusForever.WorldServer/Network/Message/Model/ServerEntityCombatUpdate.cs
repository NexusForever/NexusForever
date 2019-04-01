using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityCombatUpdate, MessageDirection.Server)]
    class ServerEntityCombatUpdate : IWritable
    {
        public uint Guid { get; set; }
        public bool CombatState { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(CombatState);
        }
    }
}
