using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestLuaEvent)]
    public class ServerQuestLuaEvent : IWritable
    {
        public enum LuaEventType
        {
            Int     = 0,
            String  = 1,
            Unit    = 2,
            Bool    = 3,
            Item    = 4,
            Quest   = 5,
        }

        public interface ILuaEventData : IWritable
        {
            public LuaEventType Type { get; }
        }

        public class LuaEventDataInt : ILuaEventData, IWritable
        {
            public LuaEventType Type => LuaEventType.Int;
            public int Int { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Int);
            }
        }

        public class LuaEventDataString : ILuaEventData, IWritable
        {
            public LuaEventType Type => LuaEventType.String;
            public string Text { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.WriteStringWide(Text);
            }
        }

        public class LuaEventDataBool : ILuaEventData, IWritable
        {
            public LuaEventType Type => LuaEventType.Bool;
            public bool Bool { get; set; }
    
            public void Write(GamePacketWriter writer)
            {
                writer.Write(Bool);
            }
        }

        public class LuaEventDataItem : ILuaEventData, IWritable
        {
            public LuaEventType Type => LuaEventType.Item;
            public uint Item2Id { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Item2Id, 18u);
            }
        }

        public class LuaEventDataQuest : ILuaEventData, IWritable
        {
            public LuaEventType Type => LuaEventType.Quest;
            public uint QuestId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(QuestId, 15u);
            }
        }

        public ushort LuaEventId { get; set; }
        public List<ILuaEventData> LuaEvents { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LuaEventId);
            writer.Write(LuaEvents.Count);
            foreach (ILuaEventData luaEvent in LuaEvents)
            {
                writer.Write(luaEvent.Type, 32u);
                luaEvent.Write(writer);
            }
        }
    }
}
