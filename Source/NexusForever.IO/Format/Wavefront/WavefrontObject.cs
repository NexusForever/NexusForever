using System.Numerics;

namespace NexusForever.IO.Format.Wavefront
{
    public class WavefrontObject : ITextWritable
    {
        public string Name { get; set; }
        public List<Vector3> Vertices { get; set; } = [];
        public List<uint> Indices { get; set; } = [];

        public void Write(StreamWriter writer)
        {
            writer.WriteLine($"o {Name}");

            foreach (Vector3 vertex in Vertices)
                writer.WriteLine($"v {vertex.X} {vertex.Y} {vertex.Z}");

            for (int i = 0; i < Indices.Count; i += 3)
                writer.WriteLine($"f {Indices[i] + 1} {Indices[i + 1] + 1} {Indices[i + 2] + 1}");
        }
    }
}
