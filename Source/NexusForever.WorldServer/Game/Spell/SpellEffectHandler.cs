using System;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Spell
{
    public delegate void SpellEffectDelegate(Spell spell, UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info);

    public partial class Spell
    {
        [SpellEffectHandler(SpellEffectType.Damage)]
        private void HandleEffectDamage(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            // TODO: calculate damage
            info.AddDamage((DamageType)info.Entry.DamageType, 1337);
        }

        [SpellEffectHandler(SpellEffectType.Proxy)]
        private void HandleEffectProxy(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            target.CastSpell(info.Entry.DataBits00, new SpellParameters
            {
                ParentSpellInfo        = parameters.SpellInfo,
                RootSpellInfo          = parameters.RootSpellInfo,
                UserInitiatedSpellCast = false
            });
        }

        [SpellEffectHandler(SpellEffectType.SummonMount)]
        private void HandleEffectSummonMount(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            // TODO: handle NPC mounting?
            if (!(target is Player player))
                return;

            if (player.VehicleGuid != 0u)
                return;

            var mount = new Mount(player, parameters.SpellInfo.Entry.Id, info.Entry.DataBits00, info.Entry.DataBits01, info.Entry.DataBits04);
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

        [SpellEffectHandler(SpellEffectType.Teleport)]
        private void HandleEffectTeleport(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            WorldLocation2Entry locationEntry = GameTableManager.WorldLocation2.GetEntry(info.Entry.DataBits00);
            if (locationEntry == null)
                return;

            if (target is Player player)
                player.TeleportTo((ushort)locationEntry.WorldId, locationEntry.Position0, locationEntry.Position1, locationEntry.Position2);
        }

        [SpellEffectHandler(SpellEffectType.RapidTransport)]
        private void HandleEffectRapidTransport(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            TaxiNodeEntry taxiNode = GameTableManager.TaxiNode.GetEntry(parameters.TaxiNode);
            if (taxiNode == null)
                return;

            WorldLocation2Entry worldLocation = GameTableManager.WorldLocation2.GetEntry(taxiNode.WorldLocation2Id);
            if (worldLocation == null)
                return;

            if (!(target is Player player))
                return;

            var rotation = new Quaternion(worldLocation.Facing0, worldLocation.Facing0, worldLocation.Facing2, worldLocation.Facing3);
            player.Rotation = rotation.ToEulerDegrees();
            player.TeleportTo((ushort)worldLocation.WorldId, worldLocation.Position0, worldLocation.Position1, worldLocation.Position2);
        }

        [SpellEffectHandler(SpellEffectType.UnlockPetFlair)]
        private void HandleEffectUnlockPetFlair(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            if (!(target is Player player))
                return;

            player.PetCustomisationManager.UnlockFlair((ushort)info.Entry.DataBits00);
        }

        [SpellEffectHandler(SpellEffectType.SummonVanityPet)]
        private void HandleEffectSummonVanityPet(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            if (!(target is Player player))
                return;

            var vanityPet = new VanityPet(player, info.Entry.DataBits00);
	        player.Map.EnqueueAdd(vanityPet, player.Position);
        }
    }
}
