using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientGuildHolomarkUpdate)]
    public class ClientGuildHolomarkUpdate : IReadable
    {
        public bool LeftHidden { get; private set; }
        public bool RightHidden { get; private set; }
        public bool BackHidden { get; private set; }
        public bool DistanceNear { get; private set; }

        public void Read(GamePacketReader reader)
        {
            LeftHidden   = reader.ReadBit();
            RightHidden  = reader.ReadBit();
            BackHidden   = reader.ReadBit();
            DistanceNear = reader.ReadBit();
        }
    }
}
