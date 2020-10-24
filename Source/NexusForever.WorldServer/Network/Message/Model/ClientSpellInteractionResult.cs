using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Social;


namespace NexusForever.WorldServer.Network.Message.Model
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
