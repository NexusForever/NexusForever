using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientSummonVanityPet)]
    public class ClientSummonVanityPet : IReadable
    {
        public uint Spell4BaseId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Spell4BaseId = reader.ReadUInt(18);
        }
    }
}
