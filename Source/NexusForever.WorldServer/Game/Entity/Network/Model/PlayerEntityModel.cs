using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Guild.Static;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class PlayerEntityModel : IEntityModel
    {
        public ulong Id { get; set; }
        public ushort RealmId { get; set; }
        public string Name { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Sex Sex { get; set; }
        public ulong GroupId { get; set; }
        public List<uint> PetIdList { get; } = new List<uint>();
        public string GuildName { get; set; }
        public GuildType GuildType { get; set; }
        public List<ulong> GuildIds { get; set; } = new List<ulong>(); // Only appears in sniffs when user has a guild, assume related to guild as well. Guild members?
        public List<float> Bones { get; set; } = new List<float>();
        public PvPFlag PvPFlag { get; set; }
        public byte Unknown4C { get; set; }
        public ushort Title { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Id);
            writer.Write(RealmId, 14);
            writer.WriteStringWide(Name);
            writer.Write(Race, 5);
            writer.Write(Class, 5);
            writer.Write(Sex, 2);
            writer.Write(GroupId);

            writer.Write((byte)PetIdList.Count);
            PetIdList.ForEach(e => writer.Write(e));

            writer.WriteStringWide(GuildName);
            writer.Write(GuildType, 4u);

            writer.Write((byte)GuildIds.Count, 5);
            GuildIds.ForEach(e => writer.Write(e));

            writer.Write(Bones.Count, 6);
            Bones.ForEach(e => writer.Write(e));

            writer.Write(PvPFlag, 3);
            writer.Write(Unknown4C);
            writer.Write(Title, 14);
        }
    }
}
