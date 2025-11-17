using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    public class Shortcut : IWritable
    {
        public ShortcutType ShortcutType { get; set; }
        public ItemLocation Location { get; set; } = new();
        public uint ObjectId { get; set; } // for ShortcutType.BagItem: item2Id
                                           // for ShortcutType.Macro: TODO research more
                                           // for ShortcutType.GameCommand: GameCommandType
                                           // for ShortcutType.SpellbookItem: spell4Id
                                           // for ShortcutType.Spell: spell4Id
                                           // for ShortcutType.VehicleAction: spell4Id
                                           // for ShortcutType.ScanbotScan: spell4Id
                                           // for ShortcutType.ToggleScanbot: 0 = powerslash, 1 = dash back, 2 = recharge
                                           // for ShortcutType.MiscSkill: 0 = powerslash, 1 = dash back, 2 = recharge
                                           // for ShortcutType.NonCombat: spell4Id

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ShortcutType, 4u);
            Location.Write(writer);
            writer.Write(ObjectId);
        }
    }
}
