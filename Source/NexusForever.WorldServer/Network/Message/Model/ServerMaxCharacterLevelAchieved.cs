using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMaxCharacterLevelAchieved, MessageDirection.Server)]
    public class ServerMaxCharacterLevelAchieved : IWritable
    {
        public uint Level { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Level);
        }
    }
}
