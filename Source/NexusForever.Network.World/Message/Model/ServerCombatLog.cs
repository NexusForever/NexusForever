using NexusForever.Network.Message;
using NexusForever.Network.World.Combat;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCombatLog)]
    public class ServerCombatLog : IWritable
    {
        public ICombatLog CombatLog { get; set; }

        public virtual void Write(GamePacketWriter writer)
        {
            writer.Write(CombatLog.Type, 6u);
            CombatLog.Write(writer);
        }
    } 
}
