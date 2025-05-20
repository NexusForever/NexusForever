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

        public class LuaEventData : IWritable
        {
            public LuaEventType Type { get; set; }

            public virtual void Write(GamePacketWriter writer)
            {
                writer.Write(Type);
            }
        }

        public class LuaEventData_Int : LuaEventData, IWritable
        {
            public int Int { get; set; }

            public LuaEventData_Int(int value)
            {
                Type = LuaEventType.Int;
                Int = value;
            }

            public override void Write(GamePacketWriter writer)
            {
                base.Write(writer);
                writer.Write(Int);
            }
        }

        public class LuaEventData_String : LuaEventData, IWritable
        {
            public string Text { get; set; }

            public LuaEventData_String(string text)
            {
                Type = LuaEventType.String;
                Text = text;
            }

            public override void Write(GamePacketWriter writer)
            {
                base.Write(writer);
                writer.WriteStringWide(Text);
            }
        }

        public class LuaEventData_Bool : LuaEventData, IWritable
        {
            public bool Bool { get; set; }
    
            public LuaEventData_Bool(bool value)
            {
                Type = LuaEventType.Bool;
                Bool = value;
            }

            public override void Write(GamePacketWriter writer)
            {
                base.Write(writer);
                writer.Write(Bool);
            }
        }

        public class LuaEventData_Item : LuaEventData, IWritable
        {
            public uint Item2Id { get; set; }

            public LuaEventData_Item(uint item2Id)
            {
                Type = LuaEventType.Item;
                Item2Id = item2Id;
            }

            public override void Write(GamePacketWriter writer)
            {
                base.Write(writer);
                writer.Write(Item2Id, 18u);
            }
        }

        public class LuaEventData_Quest : LuaEventData, IWritable
        {
            public uint QuestId { get; set; }

            public LuaEventData_Quest(uint questId)
            {
                Type = LuaEventType.Quest;
                QuestId = questId;
            }

            public override void Write(GamePacketWriter writer)
            {
                base.Write(writer);
                writer.Write(QuestId, 15u);
            }
        }

        public ushort LuaEventId { get; set; }
        public List<LuaEventData> LuaEvents { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(LuaEventId);
            writer.Write(LuaEvents.Count);
            foreach (var luaEvent in LuaEvents)
            {
                luaEvent.Write(writer);
            }
        }
    }
}
