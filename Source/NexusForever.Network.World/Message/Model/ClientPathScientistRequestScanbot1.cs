using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathScientistRequestScanbot)]
    public class ClientPathScientistRequestScanbot1 : IReadable
    {
        public uint ScanbotProfile { get; private set; }
        public bool Unknown { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ScanbotProfile = reader.ReadUInt();
            Unknown = reader.ReadBit();
        }
    }
}
