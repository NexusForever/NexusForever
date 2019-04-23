using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server06B9)]
    public class Server06B9 : IWritable
    {
        public ushort Unknown0 { get; set; } // Probably Episode ID
        public uint[] Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 14);
            for (uint i = 0u; i < Unknown1.Length; i++)
                writer.Write(Unknown1[i]);
        }
    }
}
