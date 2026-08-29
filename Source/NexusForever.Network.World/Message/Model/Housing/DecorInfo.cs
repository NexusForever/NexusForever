using NexusForever.Network.Message;
using System.Numerics;
using NexusForever.Game.Static.Housing;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    public class DecorInfo : IReadable
    {
        public Identity TargetResidence { get; } = new();
        public ulong DecorId { get; private set; }
        public DecorType DecorType { get; private set; }
        public ushort MannequinCostumeIndex { get; private set; }
        public ushort MannequinPoseId { get; private set; }
        public uint RecordFlags { get; private set; }
        public uint HookIndex { get; private set; }
        public uint PlotIndex { get; private set; }
        public float Scale { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public uint DecorInfoId { get; private set; }
        public uint ActivePropUnitId { get; private set; }
        public ulong ParentDecorId { get; private set; }
        public ushort ColourShiftId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            TargetResidence.Read(reader);
            DecorId          = reader.ReadULong();
            DecorType        = reader.ReadEnum<DecorType>(32u);

            uint temp = reader.ReadUInt();
            MannequinCostumeIndex = (ushort)(temp & 0xFFFF);
            MannequinPoseId = (ushort)(temp >> 16);
            
            RecordFlags      = reader.ReadUInt();
            HookIndex        = reader.ReadUInt();
            PlotIndex        = reader.ReadUInt();
            Scale            = reader.ReadSingle();
            Position         = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Rotation         = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            DecorInfoId      = reader.ReadUInt();
            ActivePropUnitId = reader.ReadUInt();
            ParentDecorId    = reader.ReadULong();
            ColourShiftId    = reader.ReadUShort(14u);
        }
    }
}
