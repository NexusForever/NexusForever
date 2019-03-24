using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server092C, MessageDirection.Server)]
    public class Server092C : IWritable
    {
        public class Variable : IWritable
        {
            public byte Id { get; set; }
            public byte Type { get; set; }
            public float Value { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Id, 6u);
                writer.Write(0u);
                writer.Write(Type, 2u);

                switch (Type)
                {
                    case 0:
                    case 2:
                        writer.Write(Value);
                        break;
                    case 1:
                        writer.Write((uint)Value);
                        break;
                }

                writer.Write((byte)0);
            }
        }

        public List<Variable> Variables { get; set; } = new List<Variable>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write((byte)Variables.Count);
            Variables.ForEach(u => u.Write(writer));
        }
    }
}
