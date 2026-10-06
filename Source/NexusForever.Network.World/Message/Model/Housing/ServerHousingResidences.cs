using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingResidences)]
    public class ServerHousingResidences : IWritable
    {
        public class Residence : IWritable
        {
            public Identity ResidenceIdentity { get; set; }
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
            public uint Sky { get; set; }
            public uint Music { get; set; }
            public uint Ground { get; set; }
            public ResidenceFlags Flags { get; set; }
            public uint NeighbourHarvestSplit { get; set; }
            public uint NeighbourGardenSplit { get; set; }
            public bool ResidenceDeleted { get; set; }

            public void Write(GamePacketWriter writer)
            {
                ResidenceIdentity.Write(writer);
                writer.Write(NeighbourhoodId);
                writer.Write(CharacterIdOwner.GetValueOrDefault(0ul));
                writer.Write(GuildIdOwner.GetValueOrDefault(0ul));
                writer.Write(Type, 14u);
                writer.Write(TileId);
                writer.WriteStringWide(Name);
                writer.Write(PropertyInfoId, 32u);
                writer.Write(ResidenceInfoId);
                writer.Write(WallpaperExterior);
                writer.Write(Entryway);
                writer.Write(Roof);
                writer.Write(Door);
                writer.Write(Sky);
                writer.Write(Music);
                writer.Write(Ground);
                writer.Write(Flags, 32u);
                writer.Write(NeighbourHarvestSplit);
                writer.Write(NeighbourGardenSplit);
                writer.WriteBytes(new byte[64]); // sniffs show data here but unknown what it is for
                writer.Write(ResidenceDeleted);
            }
        }

        public List<Residence> Residences { get; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Residences.Count);
            Residences.ForEach(r => r.Write(writer));
        }
    }
}
