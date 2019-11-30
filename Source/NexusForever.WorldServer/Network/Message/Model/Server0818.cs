using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    // Seems to fire when buffs are applied from NPC-created Telegraphs, like speed boosts in Northern Wilds
    [Message(GameMessageOpcode.Server0818)]
    public class Server0818 : IWritable
    {
       public uint CastingId { get; set; }
       public TargetInfo TargetInfo { get; set; }

       public void Write(GamePacketWriter writer)
       {
           writer.Write(CastingId);
           TargetInfo.Write(writer);
       }
    }
}
