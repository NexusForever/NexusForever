using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountCurrencyGrant)]
    public class ServerAccountCurrencyGrant : IWritable
    {
        public AccountCurrency AccountCurrency { get; set; }
        public ulong Unknown0 { get; set; }
        public ulong Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            AccountCurrency.Write(writer);
            writer.Write(Unknown0);
            writer.Write(Unknown1);
        }
    }
}
