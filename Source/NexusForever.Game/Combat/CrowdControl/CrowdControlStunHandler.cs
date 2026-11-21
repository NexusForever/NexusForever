using NexusForever.Game.Abstract.Combat.CrowdControl;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell.Effect.Data;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Combat.CrowdControl;
using NexusForever.Network.World.Message.Model.Combat.CrowdControl;

namespace NexusForever.Game.Combat.CrowdControl
{
    public class CrowdControlStunHandler : ICrowdControlApplyHandler
    {
        public void Apply(IUnitEntity target, ISpellTargetEffectInfo info, ISpellEffectCCStateSetData data)
        {
            if (target is not IPlayer player)
                return;

            CCStateStunVictimGameplay[] directions =
            [
                CCStateStunVictimGameplay.Forward,
                CCStateStunVictimGameplay.Backward,
                CCStateStunVictimGameplay.Left,
                CCStateStunVictimGameplay.Right
            ];

            CCStateStunVictimGameplay direction = directions[Random.Shared.Next(0, directions.Length)];

            player.Session.EnqueueMessageEncrypted(new ServerCCStateStunDirection
            {
                Direction = direction
            });
        }
    }
}
