using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingVisit)]
    public class ClientHousingVisit : IReadable
    {
        public class Residence : IReadable
        {
            public ushort RealmId { get; private set; }
            public ulong ResidenceId { get; private set; }

            public void Read(GamePacketReader reader)
            {
                RealmId     = reader.ReadUShort(14u);
                ResidenceId = reader.ReadULong();
            }
        }

        public class Community : IReadable
        {
            public ushort RealmId { get; private set; }
            public ulong NeighbourhoodId { get; private set; }

            public void Read(GamePacketReader reader)
            {
                RealmId         = reader.ReadUShort(14u);
                NeighbourhoodId = reader.ReadULong();
            }
        }

        public ulong Unknown0 { get; private set; }
        public Residence TargetResidence { get; } = new Residence();
        public string TargetResidenceName { get; private set; }
        public Community TargetCommunity { get; } = new Community();
        public string TargetCommunityName { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadULong();
            TargetResidence.Read(reader);
            TargetResidenceName = reader.ReadWideString();
            TargetCommunity.Read(reader);
            TargetCommunityName = reader.ReadWideString();
        }
    }
}
