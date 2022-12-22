using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAmpList)]
    public class ServerAmpList : IWritable
    {
        public byte SpecIndex { get; set; }
        public List<ushort> Amps { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpecIndex, 3u);
            writer.Write((byte)Amps.Count, 7u);
            foreach (ushort amp in Amps)
                writer.Write(amp);
        }
    }
}
