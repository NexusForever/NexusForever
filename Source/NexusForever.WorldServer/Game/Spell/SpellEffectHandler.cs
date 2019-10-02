using System;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Spell
{
    public delegate void SpellEffectDelegate(Spell spell, UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info);

    public partial class Spell
    {
        [SpellEffectHandler(SpellEffectType.Damage)]
        private void HandleEffectDamage(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            // TODO: calculate damage
            if(!(info.Entry.DataBits is SpellEffectDataDamage dataDamage))
                throw new ArgumentException("info.Entry.DataBits is not of type SpellEffectDataDamage as expected");

            log.Info($"Damage called: {dataDamage.Amount}");

            info.AddDamage((DamageType)info.Entry.DamageType, (uint)Math.Round(info.Entry.ParameterValue01 * caster.Level));
        }

        [SpellEffectHandler(SpellEffectType.Proxy)]
        private void HandleEffectProxy(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            if (!(info.Entry.DataBits is SpellEffectDataProxy data))
                throw new ArgumentException("info.Entry.DataBits is not of type SpellEffectDataProxy as expected");

            log.Info($"[Proxy] Spell4Id: {data.Spell4Id}, DataBits03: {data.DataBits03}");

            target.CastSpell(data.Spell4Id, new SpellParameters
            {
                ParentSpellInfo = parameters.SpellInfo,
                RootSpellInfo = parameters.RootSpellInfo,
                UserInitiatedSpellCast = false
            });
        }

        [SpellEffectHandler(SpellEffectType.UnitPropertyModifier)]
        private void HandleEffectPropertyModifier(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            if (!(target is Player player))
                return;

            if (!(info.Entry.DataBits is SpellEffectDataUnitPropertyModifier data))
                throw new ArgumentException("info.Entry.DataBits is not of type SpellEffectDataUnitPropertyModifier as expected");

            if (data.Method == 1) // Adjust value by percent
                log.Info($"[UnitPropertyModifier] {(Property)data.Property} modification method {data.Method}. Modifying by {data.BaseValue * data.Modifier}");

            if (data.Method == 2) // Override current value (mainly used by debuffs, and NPC buffs)
                log.Info($"[UnitPropertyModifier] {(Property)data.Property} modification method {data.Method}. Modifying to {data.BaseValue}");

            if (data.Method == 3) // Adjust current value
            {
                if (data.Modifier > 0u)
                    log.Info($"[UnitPropertyModifier] {(Property)data.Property} modification method {data.Method}. Modifying by {data.BaseValue * data.Modifier}");
                else
                    log.Info($"[UnitPropertyModifier] {(Property)data.Property} modification method {data.Method}. Modifying to {data.BaseValue}");
            }

            if (data.Method == 4) // Adjust current value per stack
                log.Info($"[UnitPropertyModifier] {(Property)data.Property} modification method {data.Method}. Modifying by {data.BaseValue + data.Modifier}");

        }

        //[SpellEffectHandler(SpellEffectType.Disguise)]
        //private void HandleEffectDisguise(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    if (!(target is Player player))
        //        return;

        //    Creature2Entry creature2 = GameTableManager.Creature2.GetEntry(info.Entry.DataBits02);
        //    if (creature2 == null)
        //        return;

        //    Creature2DisplayGroupEntryEntry displayGroupEntry = GameTableManager.Creature2DisplayGroupEntry.Entries.FirstOrDefault(d => d.Creature2DisplayGroupId == creature2.Creature2DisplayGroupId);
        //    if (displayGroupEntry == null)
        //        return;

        //    player.SetDisplayInfo(displayGroupEntry.Creature2DisplayInfoId);
        //}

        [SpellEffectHandler(SpellEffectType.SummonMount)]
        private void HandleEffectSummonMount(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            // TODO: handle NPC mounting?
            if (!(target is Player player))
                return;

            if (player.VehicleGuid != 0u)
                return;

            if (!(info.Entry.DataBits is SpellEffectDataDefault data))
                throw new ArgumentException("info.Entry.DataBits is not of type SpellEffectDataDefault as expected");

            var mount = new Mount(player, parameters.SpellInfo.Entry.Id, data.DataBits00, data.DataBits01, data.DataBits04);
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

            player.Map.EnqueueAdd(mount, player.Position);

            // FIXME: also cast 52539,Riding License - Riding Skill 1 - SWC - Tier 1,34464
            // FIXME: also cast 80530,Mount Sprint  - Tier 2,36122
        }

        //[SpellEffectHandler(SpellEffectType.Teleport)]
        //private void HandleEffectTeleport(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    WorldLocation2Entry locationEntry = GameTableManager.WorldLocation2.GetEntry((uint)info.Entry.DataBits00);
        //    if (locationEntry == null)
        //        return;

        //    if (target is Player player)
        //        player.TeleportTo((ushort)locationEntry.WorldId, locationEntry.Position0, locationEntry.Position1, locationEntry.Position2);
        //}

        //[SpellEffectHandler(SpellEffectType.FullScreenEffect)]
        //private void HandleFullScreenEffect(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //}

        //[SpellEffectHandler(SpellEffectType.RapidTransport)]
        //private void HandleEffectRapidTransport(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    TaxiNodeEntry taxiNode = GameTableManager.TaxiNode.GetEntry(parameters.TaxiNode);
        //    if (taxiNode == null)
        //        return;

        //    WorldLocation2Entry worldLocation = GameTableManager.WorldLocation2.GetEntry(taxiNode.WorldLocation2Id);
        //    if (worldLocation == null)
        //        return;

        //    if (!(target is Player player))
        //        return;

        //    var rotation = new Quaternion(worldLocation.Facing0, worldLocation.Facing0, worldLocation.Facing2, worldLocation.Facing3);
        //    player.Rotation = rotation.ToEulerDegrees();
        //    player.TeleportTo((ushort)worldLocation.WorldId, worldLocation.Position0, worldLocation.Position1, worldLocation.Position2);
        //}

        //[SpellEffectHandler(SpellEffectType.LearnDyeColor)]
        //private void HandleEffectLearnDyeColor(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    if (!(target is Player player))
        //        return;

        //    player.Session.GenericUnlockManager.Unlock((ushort)info.Entry.DataBits00);
        //}

        //[SpellEffectHandler(SpellEffectType.UnlockMount)]
        //private void HandleEffectUnlockMount(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    if (!(target is Player player))
        //        return;

        //    Spell4Entry spell4Entry = GameTableManager.Spell4.GetEntry((uint)info.Entry.DataBits00);
        //    player.SpellManager.AddSpell(spell4Entry.Spell4BaseIdBaseSpell);

        //    player.Session.EnqueueMessageEncrypted(new ServerUnlockMount
        //    {
        //        Spell4Id = (uint)info.Entry.DataBits00
        //    });
        //}

        //[SpellEffectHandler(SpellEffectType.UnlockPetFlair)]
        //private void HandleEffectUnlockPetFlair(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    if (!(target is Player player))
        //        return;

        //    player.PetCustomisationManager.UnlockFlair((ushort)info.Entry.DataBits00);
        //}

        //[SpellEffectHandler(SpellEffectType.UnlockVanityPet)]
        //private void HandleEffectUnlockVanityPet(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    if (!(target is Player player))
        //        return;

        //    Spell4Entry spell4Entry = GameTableManager.Spell4.GetEntry((uint)info.Entry.DataBits00);
        //    player.SpellManager.AddSpell(spell4Entry.Spell4BaseIdBaseSpell);

        //    player.Session.EnqueueMessageEncrypted(new ServerUnlockMount
        //    {
        //        Spell4Id = (uint)info.Entry.DataBits00
        //    });
        //}

        //[SpellEffectHandler(SpellEffectType.SummonVanityPet)]
        //private void HandleEffectSummonVanityPet(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        //{
        //    if (!(target is Player player))
        //        return;

        //    // enqueue removal of existing vanity pet if summoned
        //    if (player.VanityPetGuid != null)
        //    {
        //        VanityPet oldVanityPet = player.GetVisible<VanityPet>(player.VanityPetGuid.Value);
        //        oldVanityPet?.RemoveFromMap();
        //        player.VanityPetGuid = 0u;
        //    }

        //    var vanityPet = new VanityPet(player, (uint)info.Entry.DataBits00);
        //    player.Map.EnqueueAdd(vanityPet, player.Position);
        //}
    }
}
