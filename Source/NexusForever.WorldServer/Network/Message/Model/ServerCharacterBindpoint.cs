using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCharacterBindpoint)]
    public class ServerCharacterBindpoint : IWritable
    {
        public ushort BindpointId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(BindpointId);
        }
    }
}
