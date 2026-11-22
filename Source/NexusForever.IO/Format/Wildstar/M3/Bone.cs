using System.Numerics;
using NexusForever.IO.Format.WildStar.Shared.Track;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Bone : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 352u;

        // id the bone has when the model is attached to another model, -1 if none
        public short GlobalId { get; set; }
        public ushort Flags { get; set; }
        // index of the parent bone, -1 if none
        public short ParentId { get; set; }
        public short Unknown6 { get; set; }
        public byte Unknown8 { get; set; }
        public byte Unknown9 { get; set; }
        public byte UnknownA { get; set; }
        public byte UnknownB { get; set; }

        public TrackHalfVector3Array Scale { get; } = new();
        public TrackHalfVector3Array Unknown28 { get; } = new();
        public TrackHalfVector3Array Unknown40 { get; } = new();
        public TrackHalfVector3Array Unknown58 { get; } = new();
        public TrackQuaternionShortArray Rotation { get; } = new();
        public TrackQuaternionShortArray Unknown88 { get; } = new();
        public TrackVector3Array Translation { get; } = new();
        public TrackVector3Array UnknownB8 { get; } = new();

        public Matrix4x4 Transform { get; set; }
        public Matrix4x4 InverseTransform { get; set; }
        public Vector3 Unknown150 { get; set; } // possibly position

        public void Read(BinaryReader reader, long dataOffset)
        {
            GlobalId = reader.ReadInt16();
            Flags    = reader.ReadUInt16();
            ParentId = reader.ReadInt16();
            Unknown6 = reader.ReadInt16();
            Unknown8 = reader.ReadByte();
            Unknown9 = reader.ReadByte();
            UnknownA = reader.ReadByte();
            UnknownB = reader.ReadByte();
            reader.BaseStream.Position += 4; // padding

            Scale.Read(reader, dataOffset);
            Unknown28.Read(reader, dataOffset);
            Unknown40.Read(reader, dataOffset);
            Unknown58.Read(reader, dataOffset);
            Rotation.Read(reader, dataOffset);
            Unknown88.Read(reader, dataOffset);
            Translation.Read(reader, dataOffset);
            UnknownB8.Read(reader, dataOffset);

            Transform        = reader.ReadMatrix4x4();
            InverseTransform = reader.ReadMatrix4x4();
            Unknown150       = reader.ReadVector3();
            reader.BaseStream.Position += 4; // padding
        }
    }
}
