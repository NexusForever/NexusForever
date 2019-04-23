using NexusForever.Shared.GameTable;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerCurrencyChanged)]
    public class ServerPlayerCurrencyChanged : IWritable
    {
        public byte CurrencyId { get; set; }
        public ulong Amount { get; set; }
        public uint Unknown1 { get; set; } = 0;
        public uint Unknown2 { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CurrencyId - 1, 5); //TODO Investigate
            writer.Write(Amount);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
        }
    }
}
