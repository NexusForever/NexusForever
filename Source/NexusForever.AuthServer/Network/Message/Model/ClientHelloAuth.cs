using NexusForever.Shared.GameTable.Static;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientHelloAuth, MessageDirection.Client)]
    public class ClientHelloAuth : IReadable
    {
        public class HardwareInformation : IReadable
        {
            public string CpuVendor { get; private set; }
            public string CpuName { get; private set; }
            public string CpuModel { get; private set; }
            public uint Unknown0 { get; private set; }
            public uint Unknown1 { get; private set; }
            public uint Unknown2 { get; private set; }
            public uint Unknown3 { get; private set; }
            public uint Unknown4 { get; private set; }
            public uint Unknown5 { get; private set; }
            public string GpuName { get; private set; }
            public uint Unknown6 { get; private set; }
            public uint Unknown7 { get; private set; }
            public uint Unknown8 { get; private set; }
            public uint Unknown9 { get; private set; }
            public uint Unknown10 { get; private set; }
            public uint Unknown11 { get; private set; }
            public uint Unknown12 { get; private set; }
            public uint Unknown13 { get; private set; }
            public uint Unknown14 { get; private set; }

            public void Read(GamePacketReader reader)
            {
                CpuVendor = reader.ReadWideString();
                CpuName   = reader.ReadWideString();
                CpuModel  = reader.ReadWideString();
                Unknown0  = reader.ReadUInt();
                Unknown1  = reader.ReadUInt();
                Unknown2  = reader.ReadUInt();
                Unknown3  = reader.ReadUInt();
                Unknown4  = reader.ReadUInt();

                Unknown5  = reader.ReadUInt();

                GpuName   = reader.ReadWideString();
                Unknown6  = reader.ReadUInt();
                Unknown7  = reader.ReadUInt();
                Unknown8  = reader.ReadUInt();
                Unknown9  = reader.ReadUInt();
                Unknown10 = reader.ReadUInt();

                Unknown11 = reader.ReadUInt();
                Unknown12 = reader.ReadUInt();
                Unknown13 = reader.ReadUInt();
                Unknown14 = reader.ReadUInt();
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
