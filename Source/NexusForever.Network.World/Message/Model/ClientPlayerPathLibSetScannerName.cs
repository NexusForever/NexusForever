using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerPathLibSetScannerName)]
    public class ClientPlayerPathLibSetScannerName : IReadable
    {
        public PetType PetType { get; private set; }
        public uint PathScientistScanBotProfileId { get; private set; }
        public String Name { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PetType = reader.ReadEnum<PetType>(2u);
            PathScientistScanBotProfileId = reader.ReadUInt();
            Name = reader.ReadWideString();
        }
    }
}
