using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ClientVehicleInteract)]
    public class ClientVehicleInteract : IReadable
    {
        public uint InteractionUnitId   { get; private set; }
        public byte Unknown1 { get; private set; } // always 3
        public byte Unknown2 { get; private set; } // always 0

        public void Read(GamePacketReader reader)
        {
            InteractionUnitId = reader.ReadUInt();
            Unknown1 = reader.ReadByte(2u);
            Unknown2 = reader.ReadByte(3u);
        }
    }
}
