using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCostumeList)]
    public class ServerCostumeList : IWritable
    {
        public List<Costume> Costumes { get; set; } = new List<Costume>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Costumes.Count);
            Costumes.ForEach(c => c.Write(writer));
        }
    }
}
