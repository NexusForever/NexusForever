using NexusForever.Shared.GameTable.Static;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHelloAuth)]
    public class ClientHelloAuth : IReadable
    {
        public class HardwareInformation : IReadable
        {
            public class CpuInformation : IReadable
            {
                public string Manufacturer { get; private set; }
                public string Name { get; private set; }
                public string Description { get; private set; }
                public uint Family { get; private set; }
                public uint Level { get; private set; }
                public uint Revision { get; private set; }
                public uint MaxClockSpeed { get; private set; }
                public uint NumberOfCores { get; private set; }

                public void Read(GamePacketReader reader)
                {
                    Manufacturer  = reader.ReadWideString();
                    Name          = reader.ReadWideString();
                    Description   = reader.ReadWideString();
                    Family        = reader.ReadUInt();
                    Level         = reader.ReadUInt();
                    Revision      = reader.ReadUInt();
                    MaxClockSpeed = reader.ReadUInt();
                    NumberOfCores = reader.ReadUInt();
                }
            }

            public class GpuInformation : IReadable
            {
                public string Name { get; private set; }
                public uint VendorId { get; private set; }
                public uint DeviceId { get; private set; }
                public uint SubSysId { get; private set; }
                public uint Revision { get; private set; }
                public uint Unknown10 { get; private set; }

                public void Read(GamePacketReader reader)
                {
                    Name      = reader.ReadWideString();
                    VendorId  = reader.ReadUInt();
                    DeviceId  = reader.ReadUInt();
                    SubSysId  = reader.ReadUInt();
                    Revision  = reader.ReadUInt();
                    Unknown10 = reader.ReadUInt();
                }
            }

            public CpuInformation Cpu { get; } = new CpuInformation();
            public uint MemoryPhysical { get; private set; }
            public GpuInformation Gpu { get; } = new GpuInformation();
            public uint Architecture { get; private set; }
            public uint OsVersion { get; private set; }
            public uint ServicePack { get; private set; }
            public uint ProductType { get; private set; }

            public void Read(GamePacketReader reader)
            {
                Cpu.Read(reader);
                MemoryPhysical = reader.ReadUInt();
                Gpu.Read(reader);

                Architecture = reader.ReadUInt(); // v3 | 0x10000
                OsVersion    = reader.ReadUInt(); // (dwMajorVersion << 16) | dwMinorVersion
                ServicePack  = reader.ReadUInt(); // (wServicePackMajor << 16) | wServicePackMinor
                ProductType  = reader.ReadUInt();
            }
        }

        public uint Build { get; private set; }
        public ulong Unknown8 { get; private set; }
        public string Email { get; private set; }
        public NetworkGuid Unknown208 { get; } = new NetworkGuid();
        public NetworkGuid GameToken { get; } = new NetworkGuid();
        public uint Unknown228 { get; private set; }
        public Language Language { get; private set; }
        public uint Unknown230 { get; private set; }
        public uint Unknown234 { get; private set; }
        public HardwareInformation Hardware { get; } = new HardwareInformation();
        public uint RealmDataCenterId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Build      = reader.ReadUInt();
            Unknown8   = reader.ReadULong(); // 0x1588
            Email      = reader.ReadWideStringFixed();

            Unknown208.Read(reader);
            GameToken.Read(reader);

            Unknown228 = reader.ReadUInt();
            Language = reader.ReadEnum<Language>(32u);
            Unknown230 = reader.ReadUInt();
            Unknown234 = reader.ReadUInt();

            Hardware.Read(reader);

            RealmDataCenterId = reader.ReadUInt();
        }
    }
}
