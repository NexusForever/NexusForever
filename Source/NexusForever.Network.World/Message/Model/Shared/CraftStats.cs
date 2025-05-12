using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class CraftStats : IReadable, IWritable
    {
        public Property[] StatType { get; set; } = new Property[5];
        public byte Unknown1 { get; set; }
        public byte ApSpSplit { get; set; } // Attack Power and Support Power split
        public uint Unknown2 { get; set; }

        public void Read(GamePacketReader reader)
        {
            ulong temp = reader.ReadULong();

            for (int i = 0; i < StatType.Length; i++)
            {
                StatType[i] = (Property)(temp & 0xFF);
                temp >>= 8;
            }

            temp >>= 8;
            Unknown1 = (byte)(temp & 0xFF);

            temp >>= 8;
            ApSpSplit = (byte)(temp & 0xFF);

            temp >>= 8;
            Unknown2 = (uint)(temp & 0xFFFFFFFF);
        }

        public void Write(GamePacketWriter writer)
        {
            ulong temp = 0;

            for (int i = 0; i < 5; i++)
            {
                temp |= (ulong)StatType[i];
                temp <<= 8;
            }

            temp |= (ulong)Unknown1;
            temp <<= 8;

            temp |= (ulong)ApSpSplit;
            temp <<= 8;

            temp |= (ulong)Unknown2;
            temp <<= 8;

            writer.Write(temp);
        }
    }
}
