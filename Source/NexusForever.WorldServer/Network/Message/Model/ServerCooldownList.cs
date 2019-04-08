using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCooldownList)]
    public class ServerCooldownList : IWritable
    {
        public List<Cooldown> Cooldowns { get; set; } = new List<Cooldown>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Cooldowns.Count);
            Cooldowns.ForEach(c => c.Write(writer));
        }
    }
}
