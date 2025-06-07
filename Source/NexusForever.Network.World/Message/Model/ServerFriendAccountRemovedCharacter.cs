using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    // Possibly for when an account friend deletes a character while the player is online
    [Message(GameMessageOpcode.ServerFriendAccountRemovedCharacter)]
    public class ServerFriendAccountRemovedCharacter : IWritable
    {
        public uint AccountId { get; set; }
        public TargetPlayerIdentity CharacterRemoved { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountId);
            CharacterRemoved?.Write(writer);
        }
    }
}
