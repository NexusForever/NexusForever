using NexusForever.Game.Static.Combat;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Combat
{
    public interface ICombatLog : IWritable
    {
        CombatLogType Type { get; }
    }
}
