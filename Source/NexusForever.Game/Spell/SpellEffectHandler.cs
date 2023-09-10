using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Entity;
using NexusForever.Game.Map;
using NexusForever.Game.Spell.Event;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell.Static;
using NLog;
using NLog.Fluent;

namespace NexusForever.Game.Spell
{
    public static class SpellHandler
    {
        [SpellEffectHandler(SpellEffectType.Damage)]
        public static void HandleEffectDamage(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO: calculate damage
            info.AddDamage((DamageType)info.Entry.DamageType, 1337);
        }

        [SpellEffectHandler(SpellEffectType.Proxy)]
        public static void HandleEffectProxy(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            target.CastSpell(info.Entry.DataBits00, new SpellParameters
            {
                CharacterSpell         = spell.Parameters.CharacterSpell,
                ParentSpellInfo        = spell.Parameters.SpellInfo,
                RootSpellInfo          = spell.Parameters.RootSpellInfo,
                UserInitiatedSpellCast = false
            });
        }

        [SpellEffectHandler(SpellEffectType.Disguise)]
        public static void HandleEffectDisguise(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            Creature2Entry creature2 = GameTableManager.Instance.Creature2.GetEntry(info.Entry.DataBits02);
            if (creature2 == null)
                return;

            Creature2DisplayGroupEntryEntry displayGroupEntry = GameTableManager.Instance.Creature2DisplayGroupEntry.Entries.FirstOrDefault(d => d.Creature2DisplayGroupId == creature2.Creature2DisplayGroupId);
            if (displayGroupEntry == null)
                return;

            target.DisplayInfo = displayGroupEntry.Creature2DisplayInfoId;
        }

        [SpellEffectHandler(SpellEffectType.SummonMount)]
        public static void HandleEffectSummonMount(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO: handle NPC mounting?
            if (target is not IPlayer player)
                return;

            if (!player.CanMount())
                return;

            var mount = new Mount(player, spell.Parameters.SpellInfo.Entry.Id, info.Entry.DataBits00, info.Entry.DataBits01, info.Entry.DataBits04);
            mount.EnqueuePassengerAdd(player, VehicleSeatType.Pilot, 0);

            // usually for hover boards
            /*if (info.Entry.DataBits04 > 0u)
            {
                mount.SetAppearance(new ItemVisual
                {
                    Slot      = ItemSlot.Mount,
                    DisplayId = (ushort)info.Entry.DataBits04
                });
            }*/

            var position = new MapPosition
            {
                Position = player.Position
            };

            if (player.Map.CanEnter(mount, position))
                player.Map.EnqueueAdd(mount, position);

            // FIXME: also cast 52539,Riding License - Riding Skill 1 - SWC - Tier 1,34464
            // FIXME: also cast 80530,Mount Sprint  - Tier 2,36122

            player.CastSpell(52539, new SpellParameters());
            player.CastSpell(80530, new SpellParameters());
        }

        [SpellEffectHandler(SpellEffectType.Teleport)]
        public static void HandleEffectTeleport(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            WorldLocation2Entry locationEntry = GameTableManager.Instance.WorldLocation2.GetEntry(info.Entry.DataBits00);
            if (locationEntry == null)
                return;

            if (target is IPlayer player)
                if (player.CanTeleport())
                    player.TeleportTo((ushort)locationEntry.WorldId, locationEntry.Position0, locationEntry.Position1, locationEntry.Position2);
        }

        [SpellEffectHandler(SpellEffectType.FullScreenEffect)]
        public static void HandleFullScreenEffect(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO/FIXME: Add duration into the queue so that the spell will automatically finish at the correct time. This is a workaround for Full Screen Effects.
            //events.EnqueueEvent(new Event.SpellEvent(info.Entry.DurationTime / 1000d, () => { status = SpellStatus.Finished; SendSpellFinish(); }));
        }

        [SpellEffectHandler(SpellEffectType.RapidTransport)]
        public static void HandleEffectRapidTransport(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            TaxiNodeEntry taxiNode = GameTableManager.Instance.TaxiNode.GetEntry(spell.Parameters.TaxiNode);
            if (taxiNode == null)
                return;

            WorldLocation2Entry worldLocation = GameTableManager.Instance.WorldLocation2.GetEntry(taxiNode.WorldLocation2Id);
            if (worldLocation == null)
                return;

            if (target is not IPlayer player)
                return;

            if (!player.CanTeleport())
                return;

            var rotation = new Quaternion(worldLocation.Facing0, worldLocation.Facing0, worldLocation.Facing2, worldLocation.Facing3);
            player.Rotation = rotation.ToEulerDegrees();
            player.TeleportTo((ushort)worldLocation.WorldId, worldLocation.Position0, worldLocation.Position1, worldLocation.Position2);
        }

        [SpellEffectHandler(SpellEffectType.LearnDyeColor)]
        public static void HandleEffectLearnDyeColor(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            player.Account.GenericUnlockManager.Unlock((ushort)info.Entry.DataBits00);
        }

        [SpellEffectHandler(SpellEffectType.UnlockMount)]
        public static void HandleEffectUnlockMount(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(info.Entry.DataBits00);
            player.SpellManager.AddSpell(spell4Entry.Spell4BaseIdBaseSpell);

            player.Session.EnqueueMessageEncrypted(new ServerUnlockMount
            {
                Spell4Id = info.Entry.DataBits00
            });
        }

        [SpellEffectHandler(SpellEffectType.UnlockPetFlair)]
        public static void HandleEffectUnlockPetFlair(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            player.PetCustomisationManager.UnlockFlair((ushort)info.Entry.DataBits00);
        }

        [SpellEffectHandler(SpellEffectType.UnlockVanityPet)]
        public static void HandleEffectUnlockVanityPet(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(info.Entry.DataBits00);
            player.SpellManager.AddSpell(spell4Entry.Spell4BaseIdBaseSpell);

            player.Session.EnqueueMessageEncrypted(new ServerUnlockMount
            {
                Spell4Id = info.Entry.DataBits00
            });
        }

        [SpellEffectHandler(SpellEffectType.SummonVanityPet)]
        public static void HandleEffectSummonVanityPet(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            // enqueue removal of existing vanity pet if summoned
            if (player.VanityPetGuid != null)
            {
                IVanityPet oldVanityPet = player.GetVisible<IVanityPet>(player.VanityPetGuid.Value);
                oldVanityPet?.RemoveFromMap();
                player.VanityPetGuid = 0u;
            }

            var vanityPet = new VanityPet(player, info.Entry.DataBits00);

            var position = new MapPosition
            {
                Position = player.Position
            };

            if (player.Map.CanEnter(vanityPet, position))
                player.Map.EnqueueAdd(vanityPet, position);
        }

        [SpellEffectHandler(SpellEffectType.TitleGrant)]
        public static void HandleEffectTitleGrant(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (target is not IPlayer player)
                return;

            player.TitleManager.AddTitle((ushort)info.Entry.DataBits00);
        }

        [SpellEffectHandler(SpellEffectType.Fluff)]
        public static void HandleEffectFluff(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
        }

        [SpellEffectHandler(SpellEffectType.UnitPropertyModifier)]
        public static void HandleEffectPropertyModifier(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO: I suppose these could be cached somewhere instead of generating them every single effect?
            SpellPropertyModifier modifier = 
                new SpellPropertyModifier((Property)info.Entry.DataBits00, 
                    info.Entry.DataBits01, 
                    BitConverter.UInt32BitsToSingle(info.Entry.DataBits02), 
                    BitConverter.UInt32BitsToSingle(info.Entry.DataBits03), 
                    BitConverter.UInt32BitsToSingle(info.Entry.DataBits04));
            target.AddSpellModifierProperty(modifier, spell.Parameters.SpellInfo.Entry.Id);

            // TODO: Handle removing spell modifiers

            //if (info.Entry.DurationTime > 0d)
            //    events.EnqueueEvent(new SpellEvent(info.Entry.DurationTime / 1000d, () =>
            //    {
            //        player.RemoveSpellProperty((Property)info.Entry.DataBits00, parameters.SpellInfo.Entry.Id);
            //    }));
        }

        [SpellEffectHandler(SpellEffectType.Stealth)]
        public static void HandleEffectStealth(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            // TODO: Make it so that Stealth cannot be broken by damage after 3s.
            // This is referenced by EffectId 95774. It checks a Prerequisite that you have http://www.jabbithole.com/spells/assassin-59389. If you do, it'll trigger this EffectHandler with DataBits02 set to 1 (instead of 0).
            if (info.Entry.DataBits02 == 1)
                return;

            target.AddStatus(spell.CastingId, EntityStatus.Stealth);
        }

        [SpellEffectHandler(SpellEffectType.ModifySpellCooldown)]
        public static void HandleEffectModifySpellCooldown(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (!(target is Player player))
                return;

            switch ((EffectModifySpellCooldownType)info.Entry.DataBits00)
            {
                case EffectModifySpellCooldownType.Spell4:
                    player.SpellManager.SetSpellCooldown(info.Entry.DataBits01, BitConverter.Int32BitsToSingle((int)info.Entry.DataBits02));
                    break;
                case EffectModifySpellCooldownType.SpellCooldownId:
                    player.SpellManager.SetSpellCooldownByCooldownId(info.Entry.DataBits01, BitConverter.Int32BitsToSingle((int)info.Entry.DataBits02));
                    break;
                default:
                    LogManager.GetCurrentClassLogger().Warn($"Unhandled ModifySpellCooldown Type {(EffectModifySpellCooldownType)info.Entry.DataBits00}");
                    break;
            }
        }

        [SpellEffectHandler(SpellEffectType.SpellForceRemove)]
        public static void HandleEffectSpellForceRemove(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            switch ((EffectForceSpellRemoveType)info.Entry.DataBits00)
            {
                case EffectForceSpellRemoveType.Spell4:
                    ISpell activeSpell4 = target.GetActiveSpell(i => i.Parameters.SpellInfo.Entry.Id == info.Entry.DataBits01);
                    if (activeSpell4 != null)
                        activeSpell4.Finish();
                    break;
                case EffectForceSpellRemoveType.SpellBase:
                    ISpell activeSpellBase = target.GetActiveSpell(i => i.Parameters.SpellInfo.Entry.Spell4BaseIdBaseSpell == info.Entry.DataBits01);
                    if (activeSpellBase != null)
                        activeSpellBase.Finish();
                    break;
                default:
                    LogManager.GetCurrentClassLogger().Warn($"Unhandled EffectForceSpellRemoveType Type {(EffectForceSpellRemoveType)info.Entry.DataBits00}");
                    break;
            }
        }

        [SpellEffectHandler(SpellEffectType.RavelSignal)]
        public static void HandleEffectRavelSignal(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info)
        {
            if (info.Entry.DataBits00 == 1 && info.Entry.DataBits01 == 13076) // TODO: Move to actual script system. This is used in Stalker's Stealth Ability to prevent it from executing the next Effect whcih was the Cancel Stealth proxy effect.
                spell.Parameters.ParentSpellInfo.Effects.RemoveAll(i => i.Id == 91018);
            else
                LogManager.GetCurrentClassLogger().Warn($"Unhandled spell effect {SpellEffectType.RavelSignal}");
        }
    }
}
