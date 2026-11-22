using NexusForever.IO.Format.WildStar.M3.Static;
using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Geometry : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 200u;

        public uint VertexBlockCount { get; set; }
        public ushort VertexBlockSize { get; set; }
        public VertexBlockFlags VertexBlockFlags { get; set; }
        public VertexBlockFieldType[] VertexBlockFieldType { get; set; } = new VertexBlockFieldType[11];
        public byte[] VertexBlockFieldPosition { get; set; } = new byte[11];
        public PointerByteArray Vertices { get; } = new();
        public PointerUIntArray Unknown48 { get; } = new();
        public PointerUIntArray Unknown58 { get; } = new();
        public uint IndexCount { get; set; }
        public byte IndexSize { get; set; }
        public PointerByteArray Indices { get; } = new();
        public PointerReadableArray<Mesh> Mesh { get; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            reader.ReadUInt64();
            reader.ReadBytes(16);

            VertexBlockCount = reader.ReadUInt32();
            VertexBlockSize  = reader.ReadUInt16();
            VertexBlockFlags = (VertexBlockFlags)reader.ReadUInt16();

            for (uint i = 0u; i < 11u; i++)
                VertexBlockFieldType[i] = (VertexBlockFieldType)reader.ReadByte();

            VertexBlockFieldPosition = reader.ReadBytes(11);

            reader.ReadBytes(2);

            Vertices.Read(reader, dataOffset);
            Unknown48.Read(reader, dataOffset);
            Unknown58.Read(reader, dataOffset);

            IndexCount = reader.ReadUInt32();
            IndexSize  = reader.ReadByte();
            reader.ReadBytes(3);

            Indices.Read(reader, dataOffset);
            Mesh.Read(reader, dataOffset);
        }
    }
}
