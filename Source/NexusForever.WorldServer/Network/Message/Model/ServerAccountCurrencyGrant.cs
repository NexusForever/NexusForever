using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
