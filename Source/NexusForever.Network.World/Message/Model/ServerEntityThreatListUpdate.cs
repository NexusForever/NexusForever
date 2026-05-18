using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// Values from this packet are stored in a "global" threat list which is only used for the current target.
    /// This is different from <see cref="ServerEntityThreatUpdate"/> where values are stored against the entity object in the client.
    /// </summary>
    [Message(GameMessageOpcode.ServerEntityThreatListUpdate)]
    public class ServerEntityThreatListUpdate : IWritable
    {
        public uint SrcUnitId { get; set; }
        public uint[] ThreatUnitIds { get; set; } = new uint[5];
        public uint[] ThreatLevels { get; set; } = new uint[5];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SrcUnitId);

            for (int i = 0; i < ThreatUnitIds.Length; i++)
                writer.Write(ThreatUnitIds[i]);

            for (int i = 0; i < ThreatLevels.Length; i++)
                writer.Write(ThreatLevels[i]);
        }
    }
}
