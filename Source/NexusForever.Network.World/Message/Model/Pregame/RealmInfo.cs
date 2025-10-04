using NexusForever.Game.Static.Pregame;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    public class RealmInfo : IWritable
    {
        public class AccountRealmData : IWritable
        {
            public ushort RealmId { get; set; }
            public uint CharacterCount { get; set; }
            public string LastPlayedCharacter { get; set; }
            public ulong LastPlayedTime { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RealmId, 14u);
                writer.Write(CharacterCount);
                writer.WriteStringWide(LastPlayedCharacter);
                writer.Write(LastPlayedTime);
            }
        }

        public uint RealmId { get; set; }
        public string RealmName { get; set; }
        public uint RealmNoteStringId { get; set; }
        public RealmFlag Flags { get; set; }
        public RealmType Type { get; set; }
        public RealmStatus Status { get; set; }
        public RealmPopulation Population { get; set; }
        public uint Unknown7 { get; set; }
        public byte[] Unknown8 { get; set; }
        public AccountRealmData AccountRealmInfo { get; set; }
        public ushort UnknownC { get; set; }
        public ushort UnknownD { get; set; }
        public ushort UnknownE { get; set; }
        public ushort UnknownF { get; set; }

        public virtual void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId);
            writer.WriteStringWide(RealmName);
            writer.Write(RealmNoteStringId);
            writer.Write(Flags, 32u);
            writer.Write(Type, 2u);
            writer.Write(Status, 3u);
            writer.Write(Population, 3u);
            writer.Write(Unknown7);
            writer.WriteBytes(Unknown8, 16u);
            AccountRealmInfo.Write(writer);
            writer.Write(UnknownC);
            writer.Write(UnknownD);
            writer.Write(UnknownE);
            writer.Write(UnknownF);
        }
    }
}
