using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // Originally called Eldan Augmentations. See eldanAugmentation tbl for information
    [Message(GameMessageOpcode.ServerAmpList)]
    public class ServerAmpList : IWritable
    {
        public byte SpecIndex { get; set; }
        public List<ushort> Amps { get; set; } = []; // EldanAugmentationIds

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpecIndex, 3u);
            writer.Write(Amps.Count, 7u);
            foreach (ushort amp in Amps)
            {
                writer.Write(amp);
            }
        }
    }
}
