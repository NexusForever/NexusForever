using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class Costume : IWritable
    {
        public uint Index { get; set; }
        public uint Mask { get; set; }
        public byte MannequinIndex { get; set; }
        public uint[] ItemIds { get; set; } = new uint[7];
        public int[] DyeData { get; set; } = new int[7];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Index);
            writer.Write(Mask);
            writer.Write(MannequinIndex, 2u);

            for (int i = 0; i < 7; i++)
                writer.Write(ItemIds[i]);
            for (int i = 0; i < 7; i++)
                writer.Write(DyeData[i]);
        }
    }
}
