using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerInstanceSettings)]
    public class ServerInstanceSettings : IWritable
    {
        public byte Difficulty { get; set; }
        public uint PrimeLevel { get; set; }
        // 0x01 = WorldForcesLevelScaling
        // 0x02 = ??
        // 0x04 = TransmatTeleport
        // 0x08 = HoloCryptTeleport
        public byte Flags { get; set; }
        public uint Unknown3 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Difficulty, 2u);
            writer.Write(PrimeLevel);
            writer.Write(Flags);
            writer.Write(Unknown3);
        }
    }
}
