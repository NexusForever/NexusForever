using System.Numerics;
using NexusForever.IO.Format.WildStar.Area.Static;

namespace NexusForever.IO.Format.WildStar.Area
{
    public class PropChunk : IBinaryReadable
    {
        public class Prop : IBinaryReadable
        {
            public class Placement : IBinaryReadable
            {
                public ushort MinX { get; set; }
                public ushort MinY { get; set; }
                public ushort MaxX { get; set; }
                public ushort MaxY { get; set; }

                public void Read(BinaryReader reader)
                {
                    MinX = reader.ReadUInt16();
                    MinY = reader.ReadUInt16();
                    MaxX = reader.ReadUInt16();
                    MaxY = reader.ReadUInt16();
                }
            }

            public uint Id { get; set; }
            public uint ParentId { get; set; }
            public uint ChildId { get; set; }
            public uint NextId { get; set; }
            public PropType PropType { get; set; }
            public string Path { get; set; }
            public uint Unknown6 { get; set; }
            public float Scale { get; set; }
            public Quaternion Rotation { get; set; }
            public Vector3 Position { get; set; }
            public Placement PlacementD { get; set; }
            public uint Unknown7 { get; set; }
            public uint Unknown8 { get; set; }
            public uint Unknown9 { get; set; }
            public uint Colour0 { get; set; }
            public uint Colour1 { get; set; }
            public uint Unknown10 { get; set; }
            public uint ActivePropId { get; set; }
            public int Dressing { get; set; }
            public uint Unknown12 { get; set; }

            public void Read(BinaryReader reader)
            {
                Id        = reader.ReadUInt32();
                ParentId  = reader.ReadUInt32();
                ChildId   = reader.ReadUInt32();
                NextId    = reader.ReadUInt32();
                PropType  = (PropType)reader.ReadInt32();

                uint nameOffset = reader.ReadUInt32();

                long position = reader.BaseStream.Position;
                reader.BaseStream.Position = nameOffset;

                Path = reader.ReadWCString();

                reader.BaseStream.Position = position;

                Unknown6   = reader.ReadUInt32();
                Scale      = reader.ReadSingle();
                Rotation   = reader.ReadQuaternion();
                Position   = reader.ReadVector3();
                PlacementD = new Placement();
                PlacementD.Read(reader);
                Unknown7   = reader.ReadUInt32();
                Unknown8   = reader.ReadUInt32();
                Unknown9   = reader.ReadUInt32();
                Colour0    = reader.ReadUInt32();
                Colour1    = reader.ReadUInt32();
                Unknown10  = reader.ReadUInt32();
                ActivePropId  = reader.ReadUInt32();
                Dressing   = reader.ReadInt32();
                Unknown12  = reader.ReadUInt32();
            }
        }

        public List<Prop> Props { get; private set; }

        public void Read(BinaryReader reader)
        {
            uint count = reader.ReadUInt32();
            Props = new List<Prop>((int)count);

            for (uint i = 0; i < count; i++)
            {
                var prop = new Prop();
                prop.Read(reader);
                Props.Add(prop);
            }
        }
    }
}
