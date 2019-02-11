using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class GhostEntityModel : IEntityModel
    {
        public ulong Id { get; set; }
        public ushort RealmId { get; set; }
        public string Name { get; set; }
        public Race Race { get; set; }
        public Class Class { get; set; }
        public Sex Sex { get; set; }
        public ulong GroupId { get; set; }
        public string GuildName { get; set; }
        public byte GuildType { get; set; }
        public List<ulong> GuildIds { get; } = new List<ulong>();
        public List<float> Bones { get; set; } = new List<float>();
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

            writer.WriteStringWide(GuildName);
            writer.Write(GuildType, 4);

            writer.Write((byte)GuildIds.Count, 5);
            GuildIds.ForEach(e => writer.Write(e));
            
            writer.Write(Bones.Count, 6);
            Bones.ForEach(e => writer.Write(e));

            writer.Write(Title, 14);
        }
    }
}
