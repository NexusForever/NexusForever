using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server06BF, MessageDirection.Server)]
    public class Server06BF : IWritable
    {
        public ushort Unknown0 { get; set; }
        public ushort Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 15);
            writer.Write(Unknown1, 14);
        }
    }
}
