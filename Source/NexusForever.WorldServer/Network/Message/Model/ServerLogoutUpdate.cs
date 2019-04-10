using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerLogoutUpdate)]
    public class ServerLogoutUpdate : IWritable
    {
        public class SignatureBonuses : IWritable
        {
            public uint Xp { get; set; }
            public uint ElderPoints { get; set; }
            public ulong[] Currencies { get; set; }
            public ulong[] AccountCurrencies { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Xp);
                writer.Write(ElderPoints);
                writer.Write(Currencies, 15u);
                writer.Write(AccountCurrencies, 19u);
            }
        }

        public uint TimeTillLogout { get; set; }
        public bool Unknown0 { get; set; }
        public SignatureBonuses SignatureBonusData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TimeTillLogout);
            writer.Write(Unknown0);
            SignatureBonusData.Write(writer);
        }
    }
}
