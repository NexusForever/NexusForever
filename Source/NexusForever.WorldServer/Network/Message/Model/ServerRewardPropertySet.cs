using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerRewardPropertySet)]
    public class ServerRewardPropertySet : IWritable
    {
        public class RewardProperty : IWritable
        {
            public class UnknownStruct : IWritable
            {
                public byte Unknown0 { get; set; } // 4
                public uint Unknown1 { get; set; }
                public byte Type { get; set; }
                public float Value { get; set; }

                public void Write(GamePacketWriter writer)
                {
                    writer.Write(Unknown0, 4u);
                    writer.Write(Unknown1);
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
                }
            }

            public RewardPropertyType Id { get; set; }
            public uint Data { get; set; }
            public RewardPropertyModifierValueType Type { get; set; }
            public float Value { get; set; }
            public List<UnknownStruct> UnknownStructs { get; set; } = new List<UnknownStruct>();

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Id, 6u);
                writer.Write(Data);
                writer.Write(Type, 2u);

                switch (Type)
                {
                    case RewardPropertyModifierValueType.AdditiveScalar:
                    case RewardPropertyModifierValueType.MultiplicativeScalar:
                        writer.Write(Value);
                        break;
                    case RewardPropertyModifierValueType.Discrete:
                        writer.Write((uint)Value);
                        break;
                }

                writer.Write(UnknownStructs.Count, 8u);
                UnknownStructs.ForEach(u => u.Write(writer));
            }
        }

        public List<RewardProperty> Properties { get; set; } = new List<RewardProperty>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write((byte)Properties.Count);
            Properties.ForEach(u => u.Write(writer));
        }
    }
}
