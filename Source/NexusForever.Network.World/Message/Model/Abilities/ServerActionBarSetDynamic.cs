using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerActionBarSetDynamic)]
    public class ServerActionBarSetDynamic : IWritable
    {
        public ShortcutSet ShortcutSet { get; set; }
        public uint UnitToScanUnitId { get; set; } // only used for scanbot shortcuts
        public List<Shortcut> Shortcuts { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ShortcutSet, 4u);
            writer.Write(UnitToScanUnitId);
            writer.Write(Shortcuts.Count);
            Shortcuts.ForEach(shortcut => writer.Write(shortcut.ObjectId));
            Shortcuts.ForEach(shortcut => writer.Write(shortcut.ShortcutType));
        }
    }
}