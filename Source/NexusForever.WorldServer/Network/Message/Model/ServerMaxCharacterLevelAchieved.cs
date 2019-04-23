using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerMaxCharacterLevelAchieved)]
    public class ServerMaxCharacterLevelAchieved : IWritable
    {
        public uint Level { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Level);
        }
    }
}
