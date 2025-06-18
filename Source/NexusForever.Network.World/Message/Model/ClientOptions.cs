using NexusForever.Game.Static.Setting;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // When Type is Casting, NewValue is a bitmask of CastingOptionFlags.
    // When Type is SharedChallenge, NewValue only indicates whether the player wants to allow shared challenges.
    [Message(GameMessageOpcode.ClientOptions)]
    public class ClientOptions : IReadable
    {
        public OptionType Type { get; private set; }
        public uint NewValue { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            Type = reader.ReadEnum<OptionType>(32u);
            NewValue = reader.ReadUInt();
        }
    }
}
