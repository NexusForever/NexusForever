using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientVehicleInteract)]
    public class ClientVehicleInteract : IReadable
    {
        public uint InteractionUnitId   { get; private set; }
        public byte Unused1 { get; private set; } // always 3, as this never varies can be ignored
        public byte Unused2 { get; private set; } // always 0, as this never varies can be ignored

        public void Read(GamePacketReader reader)
        {
            InteractionUnitId = reader.ReadUInt();
            Unused1 = reader.ReadByte(2u);
            Unused2 = reader.ReadByte(3u);
        }
    }
}
