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

        public GroupMember.UnknownStruct0[] SomeStatList = new GroupMember.UnknownStruct0[5];

        public ushort Health { get; set; }
        public ushort HealthMax { get; set; }
        public ushort ShieldCapacity { get; set; }
        public ushort ShieldCapacityMax { get; set; }
        public ushort InterruptArmor { get; set; }
        public ushort InterruptArmorMax { get; set; }
        public ushort Absorption { get; set; }
        public ushort AbsorptionMax { get; set; }
        public ushort Focus { get; set; }
        public ushort BaseFocusPool { get; set; }
        public ushort HealingAbsorb { get; set; }
        public ushort HealingAbsorbMax { get; set; }

        public uint PhasesCanBeSeen { get; set; } = 1;
        public uint PhasesCanSee { get; set; } = 1;
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
                SomeStatList[i] = new GroupMember.UnknownStruct0();
                SomeStatList[i].Write(writer);
            }

            writer.Write(Health);
            writer.Write(HealthMax);
            writer.Write(ShieldCapacity);
            writer.Write(ShieldCapacityMax);
            writer.Write(InterruptArmor);
            writer.Write(InterruptArmorMax);
            writer.Write(Absorption);
            writer.Write(AbsorptionMax);
            writer.Write(Focus);
            writer.Write(BaseFocusPool);
            writer.Write(HealingAbsorb);
            writer.Write(HealingAbsorbMax);

            writer.Write(PhasesCanBeSeen);
            writer.Write(PhasesCanSee);
            writer.Write(Path, 3);
        }
    }
}
