using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerActionSet)]
    public class ServerActionSet : IWritable
    {
        public class Action : IWritable
        {
            public ShortcutType ShortcutType { get; set; }
            public ItemLocation Location { get; set; } = new();
            public uint ObjectId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ShortcutType, 4u);
                Location.Write(writer);
                writer.Write(ObjectId);
            }
        }

        public byte SpecIndex { get; set; }
        public byte Unlocked { get; set; } // Set to 1 to unlock spec index
        public LimitedActionSetResult Result { get; set; }
        public List<Action> Actions { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpecIndex, 3u);
            writer.Write(Unlocked, 2u);
            writer.Write(Result, 6u);
            writer.Write(Actions.Count, 6u);
            Actions.ForEach(e => e.Write(writer));
        }
    }
}
