using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class GuildMember : IWritable
    {
        public ushort Realm { get; set; }
        public ulong CharacterId { get; set; }
        public uint Rank { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; } // 2
        public Class Class { get; set; }
        public Path Path { get; set; }
        public uint Level { get; set; }
        public float LastLogoutTimeDays { get; set; }
        public uint PvpWins { get; set; }
        public uint PvpLosses { get; set; }
        public uint PvpDraws { get; set; }
        public string Note { get; set; }
        public uint Unknown10 { get; set; }
        public int Unknown11 { get; set; } = -1;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Realm, 14u);
            writer.Write(CharacterId);
            writer.Write(Rank);
            writer.WriteStringWide(Name);
            writer.Write(Sex, 2u);
            writer.Write(Class, 32u);
            writer.Write(Path, 32u);
            writer.Write(Level);
            writer.Write(LastLogoutTimeDays);
            writer.Write(PvpWins);
            writer.Write(PvpLosses);
            writer.Write(PvpDraws);
            writer.WriteStringWide(Note);
            writer.Write(Unknown10);
            writer.Write(Unknown11);
        }
    }
}
