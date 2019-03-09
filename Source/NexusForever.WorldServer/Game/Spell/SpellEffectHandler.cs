using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model;
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
                ParentSpellInfo = parameters.SpellInfo,
                RootSpellInfo = parameters.RootSpellInfo,
                UserInitiatedSpellCast = false
            });
        }

        [SpellEffectHandler(SpellEffectType.SummonMount)]
        private void HandleEffectSummonMount(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            // TODO: handle NPC mounting?

            Player player = (Player)target;
            if (player.MountId != 0)
                return;

            PetType mountType = info.Entry.DataBits01 == 411 ? PetType.HoverBoard : PetType.GroundMount;
            var mount = new Mount(player, info.Entry.DataBits00, info.Entry.SpellId, mountType);

            if (mount == null)
                return;

            // usually for hover boards
            if (info.Entry.DataBits04 > 0)
            {
                mount.itemVisuals.Add(ItemSlot.Mount, new ItemVisual
                {
                    Slot = ItemSlot.Mount,
                    DisplayId = (ushort)info.Entry.DataBits04
                });
            }

            player.Map.EnqueueAdd(mount, player.Position);

            // FIXME: add itemvisuals BodyType for hover boards?

            var petCustomizations = player.PetCustomizations.SingleOrDefault(p => p.Spell4Id == mount.Spell.Id);
            if (petCustomizations != null)
            {
                foreach (var petFlairId in petCustomizations.PetFlairIds)
                {
                    if (petFlairId < 1)
                        continue;

                    var petFlair = GameTableManager.PetFlair.GetEntry(petFlairId);
                    Spell4Entry spell4Entry = GameTableManager.Spell4.GetEntry(petFlair.Spell4Id);
                    SpellBaseInfo spellBaseInfo = GlobalSpellManager.GetSpellBaseInfo(spell4Entry.Spell4BaseIdBaseSpell);
                    SpellInfo spellInfo = spellBaseInfo.GetSpellInfo((byte)spell4Entry.TierIndex);

                    mount.CastSpell(new SpellParameters
                    {
                        SpellInfo = spellInfo
                    });
                }
            }

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

        [SpellEffectHandler(SpellEffectType.SummonVanityPet)]
        private void HandleEffectSummonVanityPet(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            Player player = (Player)target;
            var vanityPet = new VanityPet(player, info.Entry.DataBits00);

            if (vanityPet == null)
                return;

            player.Map.EnqueueAdd(vanityPet, player.Position);
        }

        [SpellEffectHandler(SpellEffectType.UnlockPetFlair)]
        private void HandleEffectUnlockPetFlair(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
            Mount mount = (Mount)target;
            Player owner = mount.Map.GetEntity<Player>(mount.OwnerGuid);

            var petFlair = GameTableManager.PetFlair.GetEntry(info.Entry.DataBits00);
            var petCustomization = owner.PetCustomizations.SingleOrDefault(p => p.Spell4Id == mount.Spell.Id);

            if (petFlair == null)
                throw new ArgumentOutOfRangeException();

            // Spell(Effect) Info does not specify if the spell is cast for left or right side...
            ItemSlot slot = 0;
            for (int i = 0; i < 4; i++)
            {
                if (petCustomization.PetFlairIds[i] == petFlair.Id)
                    if (!mount.itemVisuals.ContainsKey(slot = ItemSlot.MountFront + i))
                        break;
            }
            if (slot == 0)
                return;

            int displayIndex = slot == ItemSlot.MountRight ? 1 : 0;
            ItemDisplayEntry itemDisplay = new ItemDisplayEntry();

            if (petFlair.ItemDisplayId[displayIndex] > 0)
                itemDisplay = GameTableManager.ItemDisplay.GetEntry(petFlair.ItemDisplayId[displayIndex]);

            if (itemDisplay != null)
                mount.ItemColorSetId = itemDisplay.ItemColorSetId;

            mount.itemVisuals.Add(slot, new ItemVisual
            {
                Slot = slot,
                DisplayId = (ushort)petFlair.ItemDisplayId[displayIndex]
            });

            mount.UpdateVisuals();
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

        [SpellEffectHandler(SpellEffectType.Disguise)]
        private void HandleEffectDisguise(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
        }
        [SpellEffectHandler(SpellEffectType.DisguiseOutfit)]
        private void HandleEffectDisguiseOutfit(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
        }
        [SpellEffectHandler(SpellEffectType.MimicDisguise)]
        private void HandleEffectMimicDisguise(UnitEntity target, SpellTargetInfo.SpellTargetEffectInfo info)
        {
        }
    }
}