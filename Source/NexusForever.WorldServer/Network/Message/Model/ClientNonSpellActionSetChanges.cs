using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientNonSpellActionSetChanges, MessageDirection.Client)]
    public class ClientNonSpellActionSetChanges : IReadable
    {
        public UILocation ActionBarIndex { get; private set; }
        public ShortcutType ShortcutType { get; private set; }
        public uint ObjectId { get; private set; }
        public byte Unknown { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ActionBarIndex = reader.ReadEnum<UILocation>(6u);
            ShortcutType   = reader.ReadEnum<ShortcutType>(4u);
            ObjectId       = reader.ReadUInt();
            Unknown        = reader.ReadByte(4u);
        }
    }
}
