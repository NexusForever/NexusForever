using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerItemModify)]
    public class ServerItemModify : IWritable
    {
        public ulong ItemGuid { get; set; }
        public ulong ThresholdData { get; set; }
        public uint RandomGlyphData { get; set; }
        public double RandomCircuitData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemGuid);
            writer.Write(ThresholdData);
            writer.Write(RandomGlyphData);
            writer.Write(RandomCircuitData);
        }
    }
}
