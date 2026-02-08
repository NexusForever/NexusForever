using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // Submits the currently selected AMPs so they are saved to the player's current spec.
    [Message(GameMessageOpcode.ClientCommitAmpSpec)]
    public class ClientCommitAmpSpec : IReadable
    {
        public List<ushort> Amps { get; } = [];

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt(7u);
            for (uint i = 0; i < count; i++)
            {
                Amps.Add(reader.ReadUShort());
            }
        }
    }
}
