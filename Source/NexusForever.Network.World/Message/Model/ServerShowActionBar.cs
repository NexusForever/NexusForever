using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerShowActionBar)]
    public class ServerShowActionBar : IWritable
    {
        public ShortcutSet ShortcutSet { get; set; }
        public ushort ActionBarShortcutSetId { get; set; }
        public uint Guid { get; set; }
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(ShortcutSet, 4u);
            writer.Write(ActionBarShortcutSetId, 14u);
            writer.Write(Guid);
        }
    }
}
