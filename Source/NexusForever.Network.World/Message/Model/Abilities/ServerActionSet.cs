using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    [Message(GameMessageOpcode.ServerActionSet)]
    public class ServerActionSet : IWritable
    {
        public byte SpecIndex { get; set; }
        public byte Unlocked { get; set; } // Set to 1 to unlock spec index
        public LimitedActionSetResult Result { get; set; }
        public List<Shortcut> Shortcuts { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpecIndex, 3u);
            writer.Write(Unlocked, 2u);
            writer.Write(Result, 6u);
            writer.Write(Shortcuts.Count, 6u);
            Shortcuts.ForEach(e => e.Write(writer));
        }
    }
}
