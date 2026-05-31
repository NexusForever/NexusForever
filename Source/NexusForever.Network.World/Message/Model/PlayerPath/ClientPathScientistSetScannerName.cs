using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    // While PetType has multiple values, client only has code to use this for scanbots
    [Message(GameMessageOpcode.ClientPathScientistSetScannerName)]
    public class ClientPathScientistSetScannerName : IReadable
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
