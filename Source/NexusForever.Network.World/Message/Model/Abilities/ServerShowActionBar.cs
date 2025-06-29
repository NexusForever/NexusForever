using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerShowActionBar)]
    public class ServerShowActionBar : IWritable
    {
        public ShortcutSet ShortcutSet { get; set; }
        public ushort ActionBarShortcutSetId { get; set; }
        public uint AssociatedUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ShortcutSet, 4u);
            writer.Write(ActionBarShortcutSetId, 14u);
            writer.Write(AssociatedUnitId);
        }
    }
}
