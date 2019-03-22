using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Mail.Network.Message.Model.Shared
{
    public class MailAttachment : IWritable
    {
        public uint ItemId { get; set; } // 18
        public uint Amount { get; set; }
        public uint Unknown3 { get; set; }
        public ulong Unknown4 { get; set; }
        public uint Unknown5 { get; set; }
        public ulong Unknown6 { get; set; }
        public uint Unknown7 { get; set; } // 18
        public byte[] Unknown8 { get; set; } = new byte[20];
        public byte[] Unknown9 { get; set; } = new byte[32];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ItemId, 18u);
            writer.Write(Amount);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
            writer.Write(Unknown5);
            writer.Write(Unknown6);
            writer.Write(Unknown7, 18u);

            for (uint i = 0u; i < Unknown8.Length; i++)
                writer.Write(Unknown8[i]);

            for (uint i = 0u; i < Unknown9.Length; i++)
                writer.Write(Unknown9[i]);
        }
    }
}