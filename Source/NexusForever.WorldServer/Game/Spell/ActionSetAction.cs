using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    public class ActionSetAction
    {
        public byte ShortcutType { get; set; }
        public uint ObjectId { get; set; }
        public UILocation Location { get; set; }
        public byte Tier { get; set; }
        public ActionSaveMask saveMask { get; set; }

        /// <summary>
        /// Create a new <see cref="ActionSetAction"/>.
        /// </summary>
        public ActionSetAction(byte shortcutType, uint objectId, UILocation location, byte tier = 0, bool init = false)
        {
            ShortcutType = shortcutType;
            ObjectId     = objectId;
            Location     = location;
            Tier         = tier;
            if (!init)
                saveMask = ActionSaveMask.Create;
        }
    }
}
