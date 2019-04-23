using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
