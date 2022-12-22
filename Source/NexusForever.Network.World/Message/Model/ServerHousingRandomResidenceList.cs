using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingRandomResidenceList)]
    public class ServerHousingRandomResidenceList : IWritable
    {
        public class Residence : IWritable
        {
            public ushort RealmId { get; set; }
            public ulong ResidenceId { get; set; }
            public string Owner { get; set; }
            public string Name { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RealmId, 14u);
                writer.Write(ResidenceId);
                writer.WriteStringWide(Owner);
                writer.WriteStringWide(Name);
            }
        }

        public List<Residence> Residences { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Residences.Count);
            Residences.ForEach(c => c.Write(writer));
        }
    }
}
