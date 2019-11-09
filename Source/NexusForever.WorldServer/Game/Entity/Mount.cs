using System.Linq;
using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NetworkVehiclePassenger = NexusForever.WorldServer.Network.Message.Model.Shared.VehiclePassenger;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Mount : Vehicle
    {
        public uint OwnerGuid { get; }
        public PetType MountType { get; }

        /// <summary>
        /// Display info applied to the pilot in the <see cref="ItemSlot.Mount"/> slot.
        /// </summary>
        public ItemDisplayEntry PilotDisplayInfo { get; }

        public Mount(Player owner, uint spell4Id, uint creatureId, uint vehicleId, uint itemDisplayId)
            : base(EntityType.Mount, creatureId, vehicleId, spell4Id)
        {
            OwnerGuid        = owner.Guid;
            MountType        = vehicleId == 411 ? PetType.HoverBoard : PetType.GroundMount;
            PilotDisplayInfo = GameTableManager.Instance.ItemDisplay.GetEntry(itemDisplayId);
            Rotation         = owner.Rotation;
            Position         = owner.Position;

            Creature2DisplayGroupEntryEntry displayGroupEntry = GameTableManager.Instance.Creature2DisplayGroupEntry.Entries
                .SingleOrDefault(x => x.Creature2DisplayGroupId == CreatureEntry.Creature2DisplayGroupId);
            DisplayInfo = displayGroupEntry?.Creature2DisplayInfoId ?? 0u;

            CreateFlags |= EntityCreateFlag.SpawnAnimation;
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new MountEntityModel
            {
                CreatureId    = CreatureEntry.Id,
                UnitVehicleId = (ushort)VehicleEntry.Id,
                OwnerId       = OwnerGuid,
                Passengers    = passengers
                    .Select(p => new NetworkVehiclePassenger
                    {
                        SeatType     = p.SeatType,
                        SeatPosition = p.SeatPosition,
                        UnitId       = p.Guid
                    })
                    .ToList()
            };
        }

        public override ServerEntityCreate BuildCreatePacket()
        {
            ServerEntityCreate entityCreate = base.BuildCreatePacket();
            entityCreate.Time = 904575;
            return entityCreate;
        }

        public override void OnAddToMap(BaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            CreateFlags &= ~EntityCreateFlag.SpawnAnimation;
            CreateFlags |= EntityCreateFlag.NoSpawnAnimation;
        }

        protected override void OnPassengerAdd(Player player, VehicleSeatType seatType, byte seatPosition)
        {
            if (seatType != VehicleSeatType.Pilot)
                return;

            if (PilotDisplayInfo != null)
            {
                player.SetAppearance(new ItemVisual
                {
                    Slot      = ItemSlot.Mount,
                    DisplayId = (ushort)PilotDisplayInfo.Id
                });
            }

            PetCustomisation customisation = player.PetCustomisationManager.GetCustomisation(MountType, SpellEntry.Id);
            if (customisation != null)
            {
                ItemSlot slot = ItemSlot.MountFront;
                foreach (PetFlairEntry entry in customisation)
                {
                    if (entry != null)
                    {
                        var itemVisual = new ItemVisual
                        {
                            Slot      = slot,
                            DisplayId = (ushort) (slot != ItemSlot.MountRight
                                ? entry.ItemDisplayId[0]
                                : entry.ItemDisplayId[1])
                        };

                        // hoverboards have their flair visuals added to the player
                        if (MountType == PetType.HoverBoard)
                            player.SetAppearance(itemVisual);
                        else
                            SetAppearance(itemVisual);
                    }

                    slot++;
                }
            }

            UpdateVisuals(player);
        }

        protected override void OnPassengerRemove(Player player, VehicleSeatType seatType, byte seatPosition)
        {
            if (seatType != VehicleSeatType.Pilot)
                return;

            for (ItemSlot i = ItemSlot.MountFront; i <= ItemSlot.MountRight; i++)
            {
                var itemVisual = new ItemVisual
                {
                    Slot = i
                };

                // hoverboards have their flair visuals added to the player
                if (MountType == PetType.HoverBoard)
                    player.SetAppearance(itemVisual);
                else
                    SetAppearance(itemVisual);
            }

            if (PilotDisplayInfo != null)
            {
                player.SetAppearance(new ItemVisual
                {
                    Slot = ItemSlot.Mount
                });
            }

            UpdateVisuals(player);
        }

        private void UpdateVisuals(Player player)
        {
            var visualUpdate = new ServerEntityVisualUpdate
            {
                UnitId      = MountType == PetType.GroundMount ? Guid : player.Guid,
                Race        = (byte)player.Race,
                Sex         = (byte)player.Sex,
                ItemVisuals = (PilotDisplayInfo == null ? GetAppearance() : player.GetAppearance()).ToList()
            };

            if (MountType == PetType.GroundMount)
            {
                visualUpdate.CreatureId  = CreatureEntry.Id;
                visualUpdate.DisplayInfo = DisplayInfo;
            }

            EnqueueToVisible(visualUpdate, true);
        }
    }
}
