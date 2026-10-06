using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Spell
{
    // Seems to fire when buffs are applied from NPC-created Telegraphs, like speed boosts in Northern Wilds
    [Message(GameMessageOpcode.ServerSpellExecute)]
    public class ServerSpellExecute : IWritable
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
