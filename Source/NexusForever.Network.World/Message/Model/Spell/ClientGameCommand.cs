using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ClientGameCommand)]
    public class ClientGameCommand : IReadable
    {
        public uint ClientSpellCastUniqueId { get; private set; }
        public byte AbilityItemIndex { get; private set; }
        public byte LimitedActionSetId { get; private set; }
        public uint TargetUnitId { get; private set; }
        public Position TargetPosition { get; private set; } = new Position();

        public void Read(GamePacketReader reader)
        {
            ClientSpellCastUniqueId = reader.ReadUInt();
            AbilityItemIndex = reader.ReadByte(4u);
            LimitedActionSetId = reader.ReadByte(4u);
            TargetUnitId = reader.ReadUInt();
            TargetPosition.Read(reader);
        }
    }
}
