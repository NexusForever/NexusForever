using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendAccountCharacterLevelUpdate)]
    public class ServerFriendAccountCharacterLevelUpdate : IWritable
    {
        public uint AccountId { get; set; }
        public Identity CharacterThatLeveled { get; set; }
        public uint Level { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            CharacterThatLeveled.Write(writer);
            writer.Write(Level, 8u);
        }
    }
}
