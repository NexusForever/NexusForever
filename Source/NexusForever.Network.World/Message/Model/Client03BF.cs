using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Client03BF)]
    public class Client03BF : IReadable
    {
        public byte Unknown0 { get; set; } = 0;
        public ushort RealmId { get; set; } // 14
        public ulong CharacterId { get; set; } // 64
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public ushort Unknown3 { get; set; } // 10
        public uint Unknown4 { get; set; } // 20
        public ulong Unknown5 { get; set; } // 40

        public void Read(GamePacketReader reader)
        {
            Unknown0 = reader.ReadByte();
            RealmId = reader.ReadUShort(14);
            CharacterId = reader.ReadULong(64);
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();
            Unknown3 = reader.ReadUShort(16);
            Unknown4 = reader.ReadUInt(32);
            Unknown5 = reader.ReadULong(64);
        }
    }
}
