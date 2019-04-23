using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerGenericUnlockList)]
    public class ServerGenericUnlockList : IWritable
    {
        public List<uint> Unlocks { get; set; } = new List<uint>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unlocks.Count);
            foreach (uint unlock in Unlocks)
                writer.Write(unlock);
        }
    }
}
