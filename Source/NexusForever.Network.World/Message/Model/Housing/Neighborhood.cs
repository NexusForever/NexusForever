using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Housing
{
    public class Neighbourhood : IWritable
    {
        public ulong NeighbourhoodId { get; set; }
        public ulong HousingNeighbourhoodInfoId { get; set; }
        public ushort ParentRealmId { get; set; }
        public ulong GuildIdOwner { get; set; }
        public string Name { get; set; }
        public uint Population { get; set; }
        public uint MaxPopulation { get; set; }
        public uint Cost { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(NeighbourhoodId);
            writer.Write(HousingNeighbourhoodInfoId);
            writer.Write(ParentRealmId, 14u);
            writer.Write(GuildIdOwner);
            writer.WriteStringWide(Name);
            writer.Write(Population);
            writer.Write(MaxPopulation);
            writer.Write(Cost);
        }
    }
}
