using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingProperties)]
    public class ServerHousingProperties : IWritable
    {
        public class Residence : IWritable
        {
            public ushort RealmId { get; set; }
            public ulong ResidenceId { get; set; }
            public ulong NeighbourhoodId { get; set; }
            public ulong? CharacterIdOwner { get; set; }
            public ulong? GuildIdOwner { get; set; }
            public ResidenceType Type { get; set; }
            public uint TileId { get; set; }
            public string Name { get; set; }
            public PropertyInfoId PropertyInfoId { get; set; }
            public uint ResidenceInfoId { get; set; }
            public uint WallpaperExterior { get; set; }
            public uint Entryway { get; set; }
            public uint Roof { get; set; }
            public uint Door { get; set; }
            public uint Music { get; set; }
            public uint Ground { get; set; }
            public uint Sky { get; set; }
            public ResidenceFlags Flags { get; set; }
            public uint ResourceSharing { get; set; }
            public uint GardenSharing { get; set; }
            public bool ResidenceDeleted { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RealmId, 14u);
                writer.Write(ResidenceId);
                writer.Write(NeighbourhoodId);
                writer.Write(CharacterIdOwner.GetValueOrDefault(0ul));
                writer.Write(GuildIdOwner.GetValueOrDefault(0ul));
                writer.Write(Type, 14u);
                writer.Write(TileId);
                writer.WriteStringWide(Name);
                writer.Write(PropertyInfoId, 32u);
                writer.Write(ResidenceInfoId);
                writer.Write(Roof);
                writer.Write(WallpaperExterior);
                writer.Write(Entryway);
                writer.Write(Door);
                writer.Write(Sky);
                writer.Write(Music);
                writer.Write(Ground);
                writer.Write(Flags, 32u);
                writer.Write(ResourceSharing);
                writer.Write(GardenSharing);
                writer.WriteBytes(new byte[64]);
                writer.Write(ResidenceDeleted);
            }
        }

        public List<Residence> Residences { get; } = new();
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(Residences.Count);
            Residences.ForEach(r => r.Write(writer));
        }
    }
}
