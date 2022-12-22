using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
