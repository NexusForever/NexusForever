using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAmpList, MessageDirection.Server)]
    public class ServerAmpList : IWritable
    {
        public byte SpecIndex { get; set; }
        public List<ushort> Amps { get; set; } = new List<ushort>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpecIndex, 3u);
            writer.Write((byte)Amps.Count, 7u);
            foreach (ushort amp in Amps)
                writer.Write(amp);
        }
    }
}
