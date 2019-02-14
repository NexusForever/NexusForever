using System;
using System.IO;

namespace NexusForever.Shared.IO.Map
{
    public class MapFileCell : IReadable
    {
        [Flags]
        public enum Flags
        {
            None   = 0x00,
            Area   = 0x01,
            Height = 0x02,
            Aura   = 0x04,
            Liquid = 0x08
        }

        public uint X { get; protected set; }
        public uint Y { get; protected set; }

        protected Flags flags;

        protected readonly uint[] worldAreaIds = new uint[4];

        public void Read(BinaryReader reader)
        {
            X     = reader.ReadUInt32();
            Y     = reader.ReadUInt32();
            flags = (Flags)reader.ReadUInt32();

            for (int i = 0; i < 32; i++)
            {
                Flags flag = (Flags)(1 << i);
                if ((flags & flag) == 0)
                    continue;

                switch (flag)
                {
                    case Flags.Area:
                    {
                        for (int j = 0; j < worldAreaIds.Length; j++)
                            worldAreaIds[j] = reader.ReadUInt32();
                        break;
                    }
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public uint[] GetWorldAreaIds()
        {
            return (flags & Flags.Area) != 0 ? worldAreaIds : null;
        }
    }
}
