using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientEmote)]
    public class ClientEmote : IReadable
    {
        public uint EmoteId { get; private set; }
        public uint Seed { get; set; }
        public uint TargetUnitId { get; set; }
        public bool Targeted { get; set; }
        public bool Silent { get; set; }

        public void Read(GamePacketReader reader)
        {
            EmoteId      = reader.ReadUInt(14u);
            Seed         = reader.ReadUInt();
            TargetUnitId = reader.ReadUInt();
            Targeted     = reader.ReadBit();
            Silent       = reader.ReadBit(); // Seems to be true when the Client is instructed by the server to do this
        }
    }
}
