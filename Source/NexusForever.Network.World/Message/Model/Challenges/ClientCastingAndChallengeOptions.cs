using NexusForever.Game.Static.Setting;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // When Type is Casting, Value is a bitmask of ClientOptionFlags.
    // When Type is SharedChallenge, Value only indicates whether the player wants to allow shared challenges.
    [Message(GameMessageOpcode.ClientCastingAndChallengeOptions)]
    public class ClientCastingAndChallengeOptions : IReadable
    {
        public OptionType Type { get; private set; }
        public OptionFlags Value { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<OptionType>(32u);
            Value = reader.ReadEnum<OptionFlags>(32u);
        }
    }
}
