using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerActionSet)]
    public class ServerActionSet : IWritable
    {
        public class Action : IWritable
        {
            public ShortcutType ShortcutType { get; set; }
            public ItemLocation Location { get; set; } = new ItemLocation();
            public uint ObjectId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ShortcutType, 4u);
                Location.Write(writer);
                writer.Write(ObjectId);
            }
        }

        public byte Index { get; set; }
        public byte Unknown3 { get; set; }
        public LimitedActionSetResult Result { get; set; }
        public List<Action> Actions { get; set; } = new List<Action>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Index, 3u);
            writer.Write(Unknown3, 2u);
            writer.Write(Result, 6u);
            writer.Write(Actions.Count, 6u);
            Actions.ForEach(e => e.Write(writer));
        }
    }
}
