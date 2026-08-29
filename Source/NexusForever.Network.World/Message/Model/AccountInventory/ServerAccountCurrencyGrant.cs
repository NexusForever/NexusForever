using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountCurrencyGrant)]
    public class ServerAccountCurrencyGrant : IWritable
    {
        public AccountCurrency AccountCurrency { get; set; }
        public ulong NewAmount { get; set; }
        public ulong SignatureBonusAmount { get; set; }
        public ulong EssenceBonusAmount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            AccountCurrency.Write(writer);
            writer.Write(NewAmount);
            writer.Write(SignatureBonusAmount);
            writer.Write(EssenceBonusAmount);
        }
    }
}
