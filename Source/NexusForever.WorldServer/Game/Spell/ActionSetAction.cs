using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    public class ActionSetAction
    {
        public byte ShortcutType { get; set; }
        public uint ObjectId { get; set; }
        public UILocation Location { get; set; }

        /// <summary>
        /// Create a new <see cref="ActionSetAction"/>.
        /// </summary>
        public ActionSetAction(byte shortcutType, uint objectId, UILocation location)
        {
            ShortcutType = shortcutType;
            ObjectId     = objectId;
            Location     = location;
        }
    }
}
