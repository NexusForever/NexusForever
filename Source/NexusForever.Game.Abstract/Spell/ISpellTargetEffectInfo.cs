using NexusForever.GameTable.Model;
using NexusForever.Network.World.Combat;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellTargetEffectInfo
    {
        uint EffectId { get; }
        Spell4EffectsEntry Entry { get; }
        IDamageDescription Damage { get; }
        bool DropEffect { get; set; }
        List<ICombatLog> CombatLogs { get; }

        void AddDamage(IDamageDescription damage);
        void AddCombatLog(ICombatLog combatLog);
    }
}
