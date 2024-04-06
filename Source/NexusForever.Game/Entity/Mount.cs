using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;
using NetworkVehiclePassenger = NexusForever.Network.World.Message.Model.Shared.VehiclePassenger;

namespace NexusForever.Game.Entity
{
    public class Mount : Vehicle, IMount
    {
        public uint OwnerGuid { get; }
        public PetType MountType { get; }

        /// <summary>
        /// Display info applied to the pilot in the <see cref="ItemSlot.Mount"/> slot.
        /// </summary>
        public ItemDisplayEntry PilotDisplayInfo { get; }

        public Mount(IPlayer owner, uint spell4Id, uint creatureId, uint vehicleId, uint itemDisplayId)
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

        public override void OnAddToMap(IBaseMap map, uint guid, Vector3 vector)
        {
            base.OnAddToMap(map, guid, vector);

            CreateFlags &= ~EntityCreateFlag.SpawnAnimation;
            CreateFlags |= EntityCreateFlag.NoSpawnAnimation;
        }

        protected override void OnPassengerAdd(IPlayer player, VehicleSeatType seatType, byte seatPosition)
        {
            if (seatType != VehicleSeatType.Pilot)
                return;

            if (PilotDisplayInfo != null)
                player.AddVisual(ItemSlot.Mount, (ushort)PilotDisplayInfo.Id);

            IPetCustomisation customisation = player.PetCustomisationManager.GetCustomisation(MountType, SpellEntry.Id);
            if (customisation != null)
            {
                ItemSlot slot = ItemSlot.MountFront;
                foreach (PetFlairEntry entry in customisation)
                {
                    if (entry != null)
                    {
                        ushort displayId = (ushort)(slot != ItemSlot.MountRight ? entry.ItemDisplayId[0] : entry.ItemDisplayId[1]);

                        // hoverboards have their flair visuals added to the player
                        if (MountType == PetType.HoverBoard)
                            player.AddVisual(slot, displayId);
                        else
                            AddVisual(slot, displayId);
                    }

                    slot++;
                }
            }
        }

        protected override void OnPassengerRemove(IPlayer player, VehicleSeatType seatType, byte seatPosition)
        {
            if (seatType != VehicleSeatType.Pilot)
                return;

            for (ItemSlot slot = ItemSlot.MountFront; slot <= ItemSlot.MountRight; slot++)
            {
                // hoverboards have their flair visuals added to the player
                if (MountType == PetType.HoverBoard)
                    player.RemoveVisual(slot);
                else
                    RemoveVisual(slot);
            }

            if (PilotDisplayInfo != null)
                player.RemoveVisual(ItemSlot.Mount);
        }
    }
}
