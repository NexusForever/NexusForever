using System.Numerics;
using NexusForever.IO.Format.WildStar.Shared.Array;
using NexusForever.IO.Format.WildStar.Shared.Track;

namespace NexusForever.IO.Format.WildStar.M3
{
    public class M3File : IBinaryReadable
    {
        // client hard codes this length
        public static uint DataOffset => 1584u;

        public uint Unknown08 { get; set; }

        public PointerReadableArray<Animation> Animation { get; } = new();

        public TrackUShortArray Unknown20 { get; } = new();
        public TrackByteArray Unknown38 { get; } = new();
        public TrackByteArray Unknown50 { get; } = new();
        public TrackByteArray Unknown68 { get; } = new();

        public PointerReadableArray<Unknown80> Unknown80 { get; } = new();

        public TrackByteArray Unknown90 { get; } = new();
        public TrackByteArray UnknownA8 { get; } = new();
        public TrackByteArray UnknownC0 { get; } = new();
        public TrackByteArray UnknownD8 { get; } = new();

        public PointerReadableArray<UnknownF0> UnknownF0 { get; } = new();

        public TrackUShortArray Unknown100 { get; } = new();
        public TrackUShortArray Unknown118 { get; } = new();
        public TrackByteArray Unknown130 { get; } = new();
        public TrackByteArray Unknown148 { get; } = new();
        public TrackUShortArray Unknown160 { get; } = new();
        public float Unknown178 { get; set; }

        public PointerReadableArray<Unknown180> Unknown180 { get; } = new();

        public PointerUShortArray Unknown190 { get; } = new();
        public PointerUShortArray Unknown1A0 { get; } = new();
        public PointerUShortArray Unknown1B0 { get; } = new();
        public PointerReadableArray<Unknown1C0> Unknown1C0 { get; } = new();
        public PointerUShortArray Unknown1D0 { get; } = new();
        public PointerReadableArray<Unknown1E0> Unknown1E0 { get; } = new();
        public PointerReadableArray<Unknown1F0> Unknown1F0 { get; } = new();
        public PointerUIntArray Unknown200 { get; } = new();
        public PointerUShortArray Unknown210 { get; } = new();

        public PointerReadableArray<Unknown220> Unknown220 { get; } = new();
        public PointerReadableArray<Unknown230> Unknown230 { get; } = new();
        public PointerReadableArray<Unknown240> Unknown240 { get; } = new();

        public PointerReadableArray<Geometry> Geometries { get; } = new();

        public PointerUShortArray Unknown260 { get; } = new();
        public PointerUShortArray Unknown270 { get; } = new();
        public PointerUShortArray Unknown280 { get; } = new();
        public TrackUShortArray Unknown290 { get; } = new();
        public PointerReadableArray<Unknown2A8> Unknown2A8 { get; } = new();
        public PointerReadableArray<Unknown2B8> Unknown2B8 { get; } = new();
        public PointerULongArray Unknown2C8 { get; } = new();
        public PointerUShortArray Unknown2D8 { get; }  = new();
        public PointerULongArray Unknown2E8 { get; } = new();
        public PointerReadableArray<Unknown2F8> Unknown2F8 { get; } = new();
        public PointerReadableArray<Unknown308> Unknown308 { get; } = new();
        public PointerReadableArray<Unknown318> Unknown318 { get; } = new();
        public PointerReadableArray<Unknown328> Unknown328 { get; } = new();
        public PointerUShortArray Unknown338 { get; } = new();
        public ulong Unknown348 { get; set; }
        public TrackUIntArray Unknown350 { get; } = new();
        public ulong Unknown368 { get; set; }
        public TrackUIntArray Unknown370 { get; } = new();
        public float Unknown380 { get; set; }
        public float Unknown384 { get; set; }

        public Bounds Bounds390 { get; } = new();
        public Bounds Bounds3D0 { get; } = new();
        public Bounds Bounds410 { get; } = new();
        public Bounds Bounds450 { get; } = new();

        public PointerReadableArray<Collision> Collision { get; } = new();
        public PointerReadableArray<CollisionModelSequence> CollisionModelSequence { get; } = new();

        public Bounds Bounds4B0 { get; } = new();
        public float Unknown4F0 { get; set; }
        public float Unknown4F8 { get; set; }
        public Vector3 Unknown500 { get; set; }
        public PointerReadableArray<Unknown510> Unknown510 { get; } = new();
        public PointerUIntArray Unknown520 { get; } = new();
        public PointerUIntArray Unknown530 { get; } = new();
        public PointerReadableArray<Unknown540> Unknown540 { get; } = new();
        public PointerUShortArray Unknown550 { get; } = new();
        public PointerReadableArray<Unknown560> Unknown560 { get; } = new();
        public PointerReadableArray<Unknown570> Unknown570 { get; } = new();
        public long Unknown580 { get; set; }
        public PointerReadableArray<Unknown588> Unknown588 { get; } = new();
        public PointerReadableArray<Unknown598> Unknown598 { get; } = new();
        public PointerUShortArray Unknown5A0 { get; } = new();
        public TrackUShortArray Unknown5C0 { get; } = new();
        public float Unknown5D8 { get; set; }
        public float Unknown5E0 { get; set; }
        public float Unknown5E8 { get; set; }
        public float Unknown5F0 { get; set; }
        public float Unknown5F8 { get; set; }
        public float Unknown600 { get; set; }
        public float Unknown608 { get; set; }
        public float Unknown610 { get; set; }
        public float Unknown618 { get; set; }
        public float Unknown620 { get; set; }

        public void Read(BinaryReader reader)
        {
            uint magic = reader.ReadUInt32();
            if (magic != 0x4D4F444C)
                throw new InvalidDataException();

            uint version = reader.ReadUInt32();
            if (version != 100)
                throw new InvalidDataException();

            Unknown08 = reader.ReadUInt32();
            reader.BaseStream.Position += 4; // padding

            Animation.Read(reader, DataOffset);

            Unknown20.Read(reader, DataOffset);
            Unknown38.Read(reader, DataOffset);
            Unknown50.Read(reader, DataOffset);
            Unknown68.Read(reader, DataOffset);
            
            Unknown80.Read(reader, DataOffset);
            
            Unknown90.Read(reader, DataOffset);
            UnknownA8.Read(reader, DataOffset);
            UnknownC0.Read(reader, DataOffset);
            UnknownD8.Read(reader, DataOffset);
            
            UnknownF0.Read(reader, DataOffset);
            
            Unknown100.Read(reader, DataOffset);
            Unknown118.Read(reader, DataOffset);
            Unknown130.Read(reader, DataOffset);
            Unknown148.Read(reader, DataOffset);
            Unknown160.Read(reader, DataOffset);
            
            Unknown178 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding

            Unknown180.Read(reader, DataOffset);
            
            Unknown190.Read(reader, DataOffset);
            Unknown1A0.Read(reader, DataOffset);
            Unknown1B0.Read(reader, DataOffset);
            Unknown1C0.Read(reader, DataOffset);
            Unknown1D0.Read(reader, DataOffset);
            Unknown1E0.Read(reader, DataOffset);
            Unknown1F0.Read(reader, DataOffset);
            Unknown200.Read(reader, DataOffset);
            Unknown210.Read(reader, DataOffset);
            
            Unknown220.Read(reader, DataOffset);
            Unknown230.Read(reader, DataOffset);
            Unknown240.Read(reader, DataOffset);
            
            Geometries.Read(reader, DataOffset);

            Unknown260.Read(reader, DataOffset);
            Unknown270.Read(reader, DataOffset);
            Unknown280.Read(reader, DataOffset);
            Unknown290.Read(reader, DataOffset);
            Unknown2A8.Read(reader, DataOffset);
            Unknown2B8.Read(reader, DataOffset);
            Unknown2C8.Read(reader, DataOffset);
            Unknown2D8.Read(reader, DataOffset);
            Unknown2E8.Read(reader, DataOffset);
            Unknown2F8.Read(reader, DataOffset);
            Unknown308.Read(reader, DataOffset);
            Unknown318.Read(reader, DataOffset);
            Unknown328.Read(reader, DataOffset);
            Unknown338.Read(reader, DataOffset);
            Unknown348 = reader.ReadUInt64();
            Unknown350.Read(reader, DataOffset);
            Unknown368 = reader.ReadUInt64();
            Unknown370.Read(reader, DataOffset);
            Unknown380 = reader.ReadSingle();
            Unknown384 = reader.ReadSingle();

            Bounds390.Read(reader);
            reader.BaseStream.Position += 8; // padding
            Bounds3D0.Read(reader);
            reader.BaseStream.Position += 8; // padding
            Bounds410.Read(reader);
            reader.BaseStream.Position += 8; // padding
            Bounds450.Read(reader);
            reader.BaseStream.Position += 8; // padding

            Collision.Read(reader, DataOffset);
            CollisionModelSequence.Read(reader, DataOffset);

            Bounds4B0.Read(reader);
            reader.BaseStream.Position += 8; // padding

            Unknown4F0 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding

            Unknown4F8 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding

            Unknown500 = reader.ReadVector3();
            reader.BaseStream.Position += 4; // padding

            Unknown510.Read(reader, DataOffset);
            Unknown520.Read(reader, DataOffset);
            Unknown530.Read(reader, DataOffset);
            Unknown540.Read(reader, DataOffset);
            Unknown550.Read(reader, DataOffset);
            Unknown560.Read(reader, DataOffset);
            Unknown570.Read(reader, DataOffset);

            Unknown580 = reader.ReadInt64();

            Unknown588.Read(reader, DataOffset);
            Unknown598.Read(reader, DataOffset);
            Unknown5A0.Read(reader, DataOffset);

            reader.BaseStream.Position += 8; // padding

            Unknown5C0.Read(reader, DataOffset);

            Unknown5D8 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown5E0 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown5E8 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown5F0 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown5F8 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown600 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown608 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown610 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown618 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding
            Unknown620 = reader.ReadSingle();
            reader.BaseStream.Position += 4; // padding

            reader.BaseStream.Position += 8; // padding
        }
    }
}
