using System.Collections.Generic;
using System.IO;
using NexusForever.Shared.IO;

namespace NexusForever.MapGenerator.IO.Area
{
    public class Curt : IReadable
    {
        public class Unknown
        {
            public uint Unknown0 { get; }
            public uint Unknown4 { get; }
            public uint Unknown8 { get; }
            public uint UnknownC { get; }
            public uint Offset { get; }
            public uint Unknown14 { get; }

            public Unknown(BinaryReader reader)
            {
                Unknown0  = reader.ReadUInt32();
                Unknown4  = reader.ReadUInt32();
                Unknown8  = reader.ReadUInt32();
                UnknownC  = reader.ReadUInt32();
                Offset    = reader.ReadUInt32();
                Unknown14 = reader.ReadUInt32();
            }
        }

        public List<Unknown> Unknowns { get; } = new List<Unknown>();

        public void Read(BinaryReader reader)
        {
            uint count = reader.ReadUInt32();
            for (uint i = 0; i < count; i++)
            {
                var unknown = new Unknown(reader);
                Unknowns.Add(unknown);
            }
        }
    }
}
