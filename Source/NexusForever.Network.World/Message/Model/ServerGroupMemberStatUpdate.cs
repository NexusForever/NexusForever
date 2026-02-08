using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGroupMemberStatUpdate)]
    public class ServerGroupMemberStatUpdate : IWritable
    {
        public ulong GroupId { get; set; }
        public Identity TargetPlayer { get; set; }

        public byte Level { get; set; }
        public byte EffectiveLevel { get; set; }

        public uint Unk1 { get; set; }
        public ushort GroupMemberId { get; set; }

        public GroupCharacter.UnknownStruct0[] SomeStatList = new GroupCharacter.UnknownStruct0[5];

        public float Health { get; set; }
        public float HealthMax { get; set; }
        public float Shield { get; set; }
        public float ShieldMax { get; set; }
        public float InterruptArmor { get; set; }
        public float InterruptArmorMax { get; set; }
        public float Absorption { get; set; }
        public float AbsorptionMax { get; set; }
        public float Mana { get; set; }
        public float ManaMax { get; set; }
        public float HealingAbsorb { get; set; }
        public float HealingAbsorbMax { get; set; }

        public uint PhaseFlags1 { get; set; }
        public uint PhaseFlags2 { get; set; }
        public Game.Static.Entity.Path Path { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(GroupId);
            TargetPlayer.Write(writer);

            writer.Write(Level, 7);
            writer.Write(EffectiveLevel, 7);

            writer.Write(Unk1, 17);
            writer.Write(GroupMemberId);

            for (var i = 0; i < 5; ++i)
            {
                SomeStatList[i] = new GroupCharacter.UnknownStruct0();
                SomeStatList[i].Write(writer);
            }

            writer.WritePackedFloat(Health);
            writer.WritePackedFloat(HealthMax);
            writer.WritePackedFloat(Shield);
            writer.WritePackedFloat(ShieldMax);
            writer.WritePackedFloat(InterruptArmor);
            writer.WritePackedFloat(InterruptArmorMax);
            writer.WritePackedFloat(Absorption);
            writer.WritePackedFloat(AbsorptionMax);
            writer.WritePackedFloat(Mana);
            writer.WritePackedFloat(ManaMax);
            writer.WritePackedFloat(HealingAbsorb);
            writer.WritePackedFloat(HealingAbsorbMax);

            writer.Write(PhaseFlags1);
            writer.Write(PhaseFlags2);
            writer.Write(Path, 3);
        }
    }
}
