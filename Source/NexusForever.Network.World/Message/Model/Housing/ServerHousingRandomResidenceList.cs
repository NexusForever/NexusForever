using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingRandomResidenceList)]
    public class ServerHousingRandomResidenceList : IWritable
    {
        public class Residence : IWritable
        {
            public Identity ResidenceIdentity { get; set; }
            public string Owner { get; set; }
            public string Name { get; set; }

            public void Write(GamePacketWriter writer)
            {
                ResidenceIdentity.Write(writer);
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
