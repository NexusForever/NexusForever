using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingPlugUpdate)]
    public class ClientHousingPlugUpdate : IReadable
    {
        public ushort RealmId { get; set; }
        public ulong ResidenceId { get; set; }
        public uint PlotInfo { get; set; }
        public uint PlugItem { get; set; }
        public uint PlugFacing { get; set; }
        public uint PlotFlags { get; set; }
        public PlugUpdateOperation Operation { get; set; }

        public void Read(GamePacketReader reader)
        {
            RealmId = reader.ReadUShort(14u);
            ResidenceId = reader.ReadULong();
            PlotInfo = reader.ReadUInt();
            PlugItem = reader.ReadUInt();
            PlugFacing = reader.ReadUInt();
            PlotFlags = reader.ReadUInt();
            Operation = reader.ReadEnum<PlugUpdateOperation>(3u);

            // HousingContribution related, client function that sends this looks up values from HousingContributionInfo.tbl
            reader.ReadBytes(5 * 20);
        }
    }
}
