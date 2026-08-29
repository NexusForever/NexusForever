using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientCastSpellContinuous)]
    public class ClientCastSpellContinuous : IReadable
    {
        public ushort BagIndex { get; private set; }
        public uint TargetUnitId { get; private set; }
        public bool ButtonPressed { get; private set; }

        public void Read(GamePacketReader reader)
        {
            BagIndex  = reader.ReadUShort();
            TargetUnitId      = reader.ReadUInt();
            ButtonPressed = reader.ReadBit();
        }
    }
}
