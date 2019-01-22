using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAMPList, MessageDirection.Server)]
    public class ServerAMPList : IWritable
    {
        public byte SpecIndex { get; set; }
        public List<ushort> AMPs { get; set; } = new List<ushort>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpecIndex, 3u);
            writer.Write((byte)AMPs.Count, 7u);
            foreach (ushort amp in AMPs)
                writer.Write(amp);
        }
    }
}
