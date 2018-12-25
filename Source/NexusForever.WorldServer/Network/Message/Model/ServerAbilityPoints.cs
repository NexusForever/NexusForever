using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAbilityPoints, MessageDirection.Server)]
    public class ServerAbilityPoints : IWritable
    {
        public uint AbilityPointsAvailable { get; set; }
        public uint AbilityPointsSpent { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AbilityPointsAvailable);
            writer.Write(AbilityPointsSpent);
        }
    }
}
