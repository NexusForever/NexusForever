using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ClientNonSpellActionSetChanges)]
    public class ClientNonSpellActionSetChanges : IReadable
    {
        public UILocation ActionBarIndex { get; private set; }
        public ShortcutType ShortcutType { get; private set; }
        public uint ObjectId { get; private set; } // depending on ShortcutType can be Spell4Id, Item2Id, GameCommandType
        public byte SpecIndex { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ActionBarIndex = reader.ReadEnum<UILocation>(6u);
            ShortcutType   = reader.ReadEnum<ShortcutType>(4u);
            ObjectId       = reader.ReadUInt();
            SpecIndex      = reader.ReadByte(4u);
        }
    }
}
