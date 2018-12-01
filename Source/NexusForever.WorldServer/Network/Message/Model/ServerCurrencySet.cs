using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCurrencySet, MessageDirection.Server)]
    public class ServerCurrencySet : IWritable
    {
        public byte CurrencyId { get; set; }
        public ulong Count { get; set; }
        public uint Unknown1 { get; set; } = 0;
        public uint Unknown2 { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CurrencyId, 5);
            writer.Write(Count);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
        }
    }
}
