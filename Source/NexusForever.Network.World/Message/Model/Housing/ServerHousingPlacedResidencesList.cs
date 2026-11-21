using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingPlacedResidencesList)]
    public class ServerHousingPlacedResidencesList : IWritable
    {
        public class Residence : IWritable
        {
            public Identity ResidenceIdentity { get; set; }
            public string PlayerName { get; set; }
            public uint PropertyIndex { get; set; }

            public void Write(GamePacketWriter writer)
            {
                ResidenceIdentity.Write(writer);
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
