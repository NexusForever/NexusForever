using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientHousingPlugUpdate)]
    public class ClientHousingPlugUpdate : IReadable
    {
        public TargetPlayerIdentity TargetPlayerIdentity { get; } = new();

        public void Read(GamePacketReader reader)
        {
            TargetPlayerIdentity.Read(reader);

            reader.ReadUInt();
            reader.ReadUInt();
            reader.ReadUInt();
            reader.ReadUInt();
            reader.ReadByte(3u);

            // HousingContribution related, client function that sends this looks up values from HousingContributionInfo.tbl
            reader.ReadBytes(5 * 20);
        }
    }
}
