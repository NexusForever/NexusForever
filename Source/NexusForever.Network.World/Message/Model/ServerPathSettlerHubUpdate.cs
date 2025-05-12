using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerHubUpdate)]
    public class ServerPathSettlerHubUpdate : IWritable
    {
        public uint PathSettlerHubId { get; set; }
        public uint AvenueEconomyValue { get; set; }
        public uint AvenueSecurityValue { get; set; }
        public uint AvenueQualityOfLifeValue { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PathSettlerHubId,14);
            writer.Write(AvenueEconomyValue);
            writer.Write(AvenueSecurityValue);
            writer.Write(AvenueQualityOfLifeValue);
        }
    }
}
