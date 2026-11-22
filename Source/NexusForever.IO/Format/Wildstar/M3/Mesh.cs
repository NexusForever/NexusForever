using System.Numerics;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Mesh : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 112u;

        public uint StartIndex { get; set; }
        public uint StartVertex { get; set; }
        public uint IndexCount { get; set; }
        public uint VertexCount { get; set; }
        public ushort BoneMapIndex { get; set; }
        public ushort BoneMapCount { get; set; }
        public ushort Unknown2 { get; set; }
        public short MaterialSelector { get; set; }
        public short Unknown3 { get; set; }
        public short Unknown4 { get; set; }
        public short Unknown5 { get; set; }
        public sbyte MeshGroupId { get; set; }
        public byte Unknown6 { get; set; }
        public short Unknown7 { get; set; }
        public short MeshAnatomyId { get; set; }
        public short Unknown8 { get; set; }
        public short Unknown9 { get; set; }
        public short Unknown10 { get; set; }
        public short Unknown11 { get; set; }
        public short Unknown12 { get; set; }
        public short Unknown13 { get; set; }
        public uint Colour0 { get; set; }
        public uint Colour1 { get; set; }
        public byte Unknown16 { get; set; }
        public byte Unknown17 { get; set; }
        public Vector3 BMin { get; set; }
        public Vector3 BMax { get; set; }
        public Vector3 Unknown18 { get; set; }

        public void Read(BinaryReader reader, long dataOffset)
        {
            StartIndex       = reader.ReadUInt32();
            StartVertex      = reader.ReadUInt32();
            IndexCount       = reader.ReadUInt32();
            VertexCount      = reader.ReadUInt32();
            BoneMapIndex     = reader.ReadUInt16();
            BoneMapCount     = reader.ReadUInt16();
            Unknown2         = reader.ReadUInt16();
            MaterialSelector = reader.ReadInt16();
            Unknown3         = reader.ReadInt16();
            Unknown4         = reader.ReadInt16();
            Unknown5         = reader.ReadInt16();
            MeshGroupId      = reader.ReadSByte();
            Unknown6         = reader.ReadByte();
            Unknown7         = reader.ReadInt16();
            MeshAnatomyId    = reader.ReadInt16();
            Unknown8         = reader.ReadInt16();
            Unknown9         = reader.ReadInt16();
            Unknown10        = reader.ReadInt16();
            Unknown11        = reader.ReadInt16();
            Unknown12        = reader.ReadInt16();
            Unknown13        = reader.ReadInt16();
            Colour0          = reader.ReadUInt32();
            Colour1          = reader.ReadUInt32();
            Unknown16        = reader.ReadByte();
            Unknown17        = reader.ReadByte();

            reader.BaseStream.Position += 6;

            BMin      = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadSingle();
            BMax      = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadSingle();
            Unknown18 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadSingle();
        }
    }
}
