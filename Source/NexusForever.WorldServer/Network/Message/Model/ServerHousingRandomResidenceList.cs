using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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

        public List<Residence> Residences { get; set; } = new List<Residence>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Residences.Count);
            Residences.ForEach(c => c.Write(writer));
        }
    }
}
