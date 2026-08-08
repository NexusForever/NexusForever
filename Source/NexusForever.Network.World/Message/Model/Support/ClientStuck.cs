using NexusForever.Game.Static.Support;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Support
{
    // Initiates a spell cast to unstick the player
    [Message(GameMessageOpcode.ClientStuck)]
    public class ClientStuck : IReadable
    {
        public UnstickType UnstickingType { get; private set; }
        public uint ClientSpellCastUniqueId { get; private set; } 

        public void Read(GamePacketReader reader)
        {
            UnstickingType = reader.ReadEnum<UnstickType>(3u);
            ClientSpellCastUniqueId = reader.ReadUInt();
        }
    }
}
