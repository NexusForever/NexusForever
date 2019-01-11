using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Network.Message.Model;
using Item = NexusForever.WorldServer.Game.Entity.Item;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SpellHandler
    {
        [MessageHandler(GameMessageOpcode.ClientCastSpell)]
        public static void HandlePlayerCastSpell(WorldSession session, ClientCastSpell castSpell)
        {
            // the code in the function is temporary and just for a bit of fun, it will be replaced when the underlying spell system is implemented
            Item item = session.Player.Inventory.GetItem(InventoryLocation.Ability, castSpell.BagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            UnlockedSpell spell = session.Player.SpellManager.GetSpell(item.Id);
            if (spell == null)
                throw new InvalidPacketValueException();

            // true is probably "begin casting"
            if (!castSpell.Unknown48)
                return;

            uint castingId = GlobalSpellManager.NextCastingId;
            uint castingId_dmg = 0;
            Spell4Entry spell4Entry = GameTableManager.Spell4.Entries
                .SingleOrDefault(x => x.Spell4BaseIdBaseSpell == spell.Entry.Id && x.TierIndex == spell.Tier);

            session.Player.EnqueueToVisible(new ServerSpellStart
            {
                CastingId              = castingId,
                CasterId               = session.Player.Guid,
                PrimaryTargetId        = session.Player.Guid,
                Spell4Id               = spell4Entry.Id,
                RootSpell4Id           = spell4Entry.Id,
                ParentSpell4Id         = 0,
                FieldPosition          = new Position(session.Player.Position),
                UserInitiatedSpellCast = true
            }, true);

            var targetInfo = new ServerSpellGo.TargetInfo
            {
                UnitId        = session.Player.Guid,
                TargetFlags   = 1,
                InstanceCount = 1,
                CombatResult  = 2
            };

            var targetInfo_dmg = new ServerSpellGo.TargetInfo
            {
                UnitId        = session.Player.Target,
                TargetFlags   = 1,
                InstanceCount = 1,
                CombatResult  = 2
            };

            foreach (Spell4EffectsEntry spell4EffectEntry in GameTableManager.Spell4Effects.Entries
                .Where(x => x.SpellId == spell4Entry.Id))
            {
                targetInfo.EffectInfoData.Add(new ServerSpellGo.TargetInfo.EffectInfo
                {
                    Spell4EffectId = spell4EffectEntry.Id,
                    EffectUniqueId = 4722,
                    TimeRemaining  = -1,
                });

                if (spell4EffectEntry.DamageType < 1)
                    continue;

                Spell4Entry spellDamageId = GameTableManager.Spell4.GetEntry(spell4EffectEntry.DataBits00);
                if (spellDamageId.Id<1)
                    continue;

                Spell4EffectsEntry spell4EffectEntry_dmg = GameTableManager.Spell4Effects.Entries
                    .SingleOrDefault(x => x.SpellId == spellDamageId.Id );

                if (spell4EffectEntry_dmg.Id<1)
                    continue;

                // this is like the damage info casting instance -> as seperate on retail
                if (castingId_dmg<1)
                {
                    castingId_dmg = GlobalSpellManager.NextCastingId;
                    session.Player.EnqueueToVisible(new ServerSpellStart
                    {
                        CastingId              = castingId_dmg,
                        CasterId               = session.Player.Guid,
                        PrimaryTargetId        = session.Player.Guid,
                        Spell4Id               = spellDamageId.Id,
                        RootSpell4Id           = spell4Entry.Id,
                        ParentSpell4Id         = spell4Entry.Id,
                        FieldPosition          = new Position(session.Player.Position),
                        UserInitiatedSpellCast = true
                    }, true);
                }

                var damageDesc = new ServerSpellGo.TargetInfo.EffectInfo.DamageDescription
                {
                    RawDamage = 1337,
                    RawScaledDamage = 1337,
                    AdjustedDamage = 1337,
                    CombatResult = 1,
                    DamageType = (byte)spell4EffectEntry_dmg.DamageType
                };

                targetInfo_dmg.EffectInfoData.Add(new ServerSpellGo.TargetInfo.EffectInfo
                {
                    Spell4EffectId = spell4EffectEntry_dmg.Id,
                    EffectUniqueId = 4723,
                    TimeRemaining  = -1,
                    InfoType = 1,
                    DamageDescriptionData = damageDesc
                });
            }

            session.Player.EnqueueToVisible(new ServerSpellGo
            {
                ServerUniqueId     = castingId,
                PrimaryDestination = new Position(session.Player.Position),
                Phase              = 255,
                TargetInfoData     = new List<ServerSpellGo.TargetInfo>
                {
                    targetInfo 
                }
            }, true);

            if (castingId_dmg<1)
                return;

            session.Player.EnqueueToVisible(new ServerSpellGo
            {
                ServerUniqueId     = castingId_dmg,
                PrimaryDestination = new Position(session.Player.Position),
                Phase              = 255,
                TargetInfoData     = new List<ServerSpellGo.TargetInfo>
                {
                    targetInfo_dmg
                }
            }, true);
        }
    }
}
