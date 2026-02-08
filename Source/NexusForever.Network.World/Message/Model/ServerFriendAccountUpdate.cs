using NexusForever.Game.Static.Chat;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendAccountUpdate)]
    public class ServerFriendAccountUpdate : IWritable
    {
        public uint AccountId { get; set; }
        public AccountPresenceState State { get; set; }
        List<CharacterData> Characters { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            writer.Write(State, 3u);
            writer.Write(Characters.Count);
            foreach(var character in Characters)
            {
                character.Write(writer);
            }
        }
    }
}
