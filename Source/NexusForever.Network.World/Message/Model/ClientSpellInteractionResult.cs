using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientInteractionResult)]
    public class ClientSpellInteractionResult : IReadable
    {
        public uint CastingId { get; private set; }
        public byte Result { get; private set; } // 3u
        public uint Validation { get; private set; }

        public void Read(GamePacketReader reader)
        {
            CastingId = reader.ReadUInt();
            Result = reader.ReadByte(3u);
            Validation = reader.ReadUInt();
        }
    }
}