using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientWhoRequest, MessageDirection.Client)]
    public class ClientWhoRequest : IReadable
    {
        #region off_DB3184
        public class sub_4620B0 : IReadable
        {
            public string Unk1 { get; set; }
            public uint Unk2 { get; set; }
            public uint Unk3 { get; set; }
            public uint Unk4 { get; set; }
            public uint Unk5 { get; set; }
            public uint Unk6 { get; set; }
            public void Read(GamePacketReader reader)
            {
                Unk1 = reader.ReadWideString();
                Unk2 = reader.ReadUInt(14);
                Unk3 = reader.ReadByte(3);
                Unk4 = reader.ReadUInt(14);
                Unk5 = reader.ReadUInt(15);
                Unk6 = reader.ReadUInt(14);
            }
        }
        #endregion

        public uint Unk1 { get; set; }
        public uint Unk2 { get; set; }
        public uint Unk3 { get; set; }
        public uint Unk4 { get; set; }
        public sub_4620B0 Unk5 { get; set; }
        public void Read(GamePacketReader reader)
        {
            Unk1 = reader.ReadUInt();
            Unk2 = reader.ReadUInt();

            // sub_462150
            Unk3 = reader.ReadUInt(4);
            Unk4 = reader.ReadUInt(4);
            Unk5.Read(reader);
        }
    }
}
