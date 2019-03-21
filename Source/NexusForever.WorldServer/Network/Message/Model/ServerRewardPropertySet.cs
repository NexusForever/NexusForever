using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using RewardPropertyEnum = NexusForever.WorldServer.Game.Entity.Static.RewardProperty;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRewardPropertySet, MessageDirection.Server)]
    public class ServerRewardPropertySet : IWritable
    {
        public class RewardProperty : IWritable
        {
            public RewardPropertyEnum Id { get; set; }
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

        public List<RewardProperty> Variables { get; set; } = new List<RewardProperty>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write((byte)Variables.Count);
            Variables.ForEach(u => u.Write(writer));
        }
    }
}
