using NexusForever.IO.Format.WildStar.Shared.Array;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class Collision : IBinaryReadableOffset, IStaticSize
    {
        public static uint Size => 64u;

        public PointerReadableArray<CollisionMesh> Meshes { get; set; } = new();
        public PointerVector3Array Vertices { get; } = new();
        public PointerUIntArray Indices { get; } = new();
        public PointerReadableArray<CollisionUnknown> Unknown30 { get; set; } = new();

        public void Read(BinaryReader reader, long dataOffset)
        {
            Meshes.Read(reader, dataOffset);
            Vertices.Read(reader, dataOffset);
            Indices.Read(reader, dataOffset);
            Unknown30.Read(reader, dataOffset);
        }
    }
}
