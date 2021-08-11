using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingPlacedResidencesList)]
    public class ServerHousingPlacedResidencesList : IWritable
    {
        public class Residence : IWritable
        {
            public ushort RealmId { get; set; }
            public ulong ResidenceId { get; set; }
            public string PlayerName { get; set; }
            public uint PropertyIndex { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RealmId, 14u);
                writer.Write(ResidenceId);
                writer.WriteStringWide(PlayerName);
                writer.Write(PropertyIndex);
            }
        }

        public List<Residence> Residences { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Residences.Count);
            Residences.ForEach(r => r.Write(writer));
        }
    }
}
