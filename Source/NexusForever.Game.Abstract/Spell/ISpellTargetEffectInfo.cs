using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellTargetEffectInfo
    {
        uint EffectId { get; }
        Spell4EffectsEntry Entry { get; }
        IDamageDescription Damage { get; }
        bool DropEffect { get; set; }
        List<ServerCombatLog> CombatLogs { get; }

        void AddDamage(DamageType damageType, uint damage);
        void AddDamage(IDamageDescription damage);
        void AddCombatLog(ServerCombatLog combatLog);
    }
}