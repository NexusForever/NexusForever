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
        public uint Unused1 { get; set; }
        public byte[] Unused2 { get; set; } = new byte[16];
        public AccountRealmData AccountRealmInfo { get; set; }
        public ushort Unused3 { get; set; }
        public ushort Unused4 { get; set; }
        public ushort Unused5 { get; set; }
        public ushort Unused6 { get; set; }

        public virtual void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId);
            writer.WriteStringWide(RealmName);
            writer.Write(RealmNoteStringId);
            writer.Write(Flags, 32u);
            writer.Write(Type, 2u);
            writer.Write(Status, 3u);
            writer.Write(Population, 3u);
            writer.Write(Unused1);
            writer.WriteBytes(Unused2, 16u);
            AccountRealmInfo.Write(writer);
            writer.Write(Unused3);
            writer.Write(Unused4);
            writer.Write(Unused5);
            writer.Write(Unused6);
        }
    }
}
