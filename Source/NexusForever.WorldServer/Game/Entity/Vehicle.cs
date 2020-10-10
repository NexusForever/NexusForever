using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Network;
using NexusForever.WorldServer.Game.Entity.Network.Command;
using NexusForever.WorldServer.Game.Entity.Network.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkVehiclePassenger = NexusForever.WorldServer.Network.Message.Model.Shared.VehiclePassenger;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Vehicle : WorldEntity, IEnumerable<VehiclePassenger>
    {
        public Creature2Entry CreatureEntry { get; }
        public UnitVehicleEntry VehicleEntry { get; }
        public Spell4Entry SpellEntry { get; }

        protected readonly List<VehiclePassenger> passengers = new List<VehiclePassenger>();
        private readonly Queue<VehiclePassenger> pendingAdd = new Queue<VehiclePassenger>();

        public Vehicle()
            : base(EntityType.Vehicle)
        {
            // TODO
        }

        protected Vehicle(EntityType type, uint creatureId, uint vehicleId, uint spell4Id)
            : base(type)
        {
            CreatureEntry = GameTableManager.Instance.Creature2.GetEntry(creatureId);
            VehicleEntry  = GameTableManager.Instance.UnitVehicle.GetEntry(vehicleId != 0u ? vehicleId : CreatureEntry.UnitVehicleId);
            SpellEntry    = GameTableManager.Instance.Spell4.GetEntry(spell4Id);

            // temp
            SetProperty(Property.BaseHealth, 800.0f, 800.0f);

            SetStat(Stat.Health, 800u);
            SetStat(Stat.Level, 3u);
            SetStat(Stat.Sheathed, 800u);
        }

        protected override IEntityModel BuildEntityModel()
        {
            return new VehicleEntityModel
            {
                CreatureId    = CreatureEntry.Id,
                UnitVehicleId = (ushort)VehicleEntry.Id,
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

        public override void OnRemoveFromMap()
        {
            foreach (VehiclePassenger passenger in passengers)
            {
                Player entity = GetVisible<Player>(passenger.Guid);
                PassengerRemove(entity, passenger);
            }

            base.OnRemoveFromMap();
        }

        public override void OnRelocate(Vector3 vector)
        {
            foreach (VehiclePassenger passenger in passengers)
            {
                Player entity = GetVisible<Player>(passenger.Guid);
                Map.EnqueueRelocate(entity, vector);
            }

            base.OnRelocate(vector);
        }

        public override void Update(double lastTick)
        {
            // passengers are delay added to make sure the vehicle exists at the client
            while (pendingAdd.TryDequeue(out VehiclePassenger passenger))
                PassengerAdd(passenger);
        }

        /// <summary>
        /// Return <see cref="VehiclePassenger"/> with supplied guid.
        /// </summary>
        public VehiclePassenger GetPassenger(uint guid)
        {
            return passengers.SingleOrDefault(p => p.Guid == guid);
        }

        /// <summary>
        /// Return <see cref="VehiclePassenger"/> with supplied <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        public VehiclePassenger GetPassenger(VehicleSeatType seatType, byte seatPosition)
        {
            return passengers.SingleOrDefault(p => p.SeatType == seatType && p.SeatPosition == seatPosition);
        }

        /// <summary>
        /// Enqueue <see cref="Player"/> to be added as a passenger with supplied <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        public void EnqueuePassengerAdd(Player player, VehicleSeatType seatType, byte seatPosition)
        {
            if (seatType >= VehicleSeatType.Invalid)
                throw new ArgumentOutOfRangeException();

            byte passengerCount = (byte)passengers.Count(p => p.SeatType == seatType);
            switch (seatType)
            {
                case VehicleSeatType.Pilot:
                    if (passengerCount >= VehicleEntry.NumberPilots)
                        throw new ArgumentException();
                    break;
                case VehicleSeatType.Passenger:
                    if (passengerCount >= VehicleEntry.NumberPassengers)
                        throw new ArgumentException();
                    break;
                case VehicleSeatType.Gunner:
                    if (passengerCount >= VehicleEntry.NumberGunners)
                        throw new ArgumentException();
                    break;
            }

            if (GetPassenger(seatType, seatPosition) != null)
                throw new InvalidOperationException();

            if (pendingAdd.Any(p => p.Guid == player.Guid))
                throw new InvalidOperationException();

            pendingAdd.Enqueue(new VehiclePassenger(seatType, seatPosition, player.Guid));
        }

        private void PassengerAdd(VehiclePassenger passenger)
        {
            // possible for a player to no longer be visible due to delayed add
            Player player = GetVisible<Player>(passenger.Guid);
            if (player == null)
                return;

            // TODO: research this...
            player.Session.EnqueueMessageEncrypted(new Server08B3
            {
                MountGuid = Guid,
                Unknown0  = 0,
                Unknown1  = true
            });

            // TODO: research this, something UI related
            player.Session.EnqueueMessageEncrypted(new Server0237
            {
            });

            // TODO: research this...
            // Kirmmin: This is used on both mounting up and dismounting
            player.Session.EnqueueMessageEncrypted(new Server0639
            {
            });

            // sets vehicle guid, seat type and seat position to local self entity at client
            // might not be correct as ServerVehiclePassengerAdd does this too, used for changing seats instead?
            player.Session.EnqueueMessageEncrypted(new Server089B
            {
                Self         = passenger.Guid,
                Vehicle      = Guid,
                SeatType     = passenger.SeatType,
                SeatPosition = passenger.SeatPosition
            });

            EnqueueToVisible(new ServerVehiclePassengerAdd
            {
                Self         = Guid,
                SeatType     = passenger.SeatType,
                SeatPosition = passenger.SeatPosition,
                UnitId       = passenger.Guid
            }, true);

            if (passenger.SeatType == VehicleSeatType.Pilot)
            {
                player.SetControl(this);

                EnqueueToVisible(new ServerEntityFaction
                {
                    UnitId  = Guid,
                    Faction = (ushort)player.Faction1
                }, true);

                Faction1 = player.Faction1;
                Faction2 = player.Faction2;
            }

            player.VehicleGuid = Guid;
            player.MovementManager.SetPosition(Vector3.Zero);
            player.MovementManager.SetRotation(Vector3.Zero);
            player.MovementManager.BroadcastCommands();

            passengers.Add(passenger);
            OnPassengerAdd(player, passenger.SeatType, passenger.SeatPosition);
        }

        /// <summary>
        /// Invoked when <see cref="Player"/> is added as a passenger to <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        protected virtual void OnPassengerAdd(Player player, VehicleSeatType seatType, byte seatPosition)
        {
            // deliberately empty
        }

        /// <summary>
        /// Remove <see cref="Player"/> as a passenger.
        /// </summary>
        public void PassengerRemove(Player player)
        {
            VehiclePassenger passenger = GetPassenger(player.Guid);
            if (passenger == null)
                throw new ArgumentException();

            PassengerRemove(player, passenger);
        }

        private void PassengerRemove(Player player, VehiclePassenger passenger)
        {
            player.VehicleGuid = 0;
            player.MovementManager.SetPosition(Position);
            player.MovementManager.SetRotation(Rotation);
            player.MovementManager.BroadcastCommands();


            if (passenger.SeatType == VehicleSeatType.Pilot)
                player.SetControl(player);

            passengers.Remove(passenger);
            OnPassengerRemove(player, passenger.SeatType, passenger.SeatPosition);

            EnqueueToVisible(new ServerVehiclePassengerRemove
            {
                Self      = Guid,
                Passenger = passenger.Guid
            }, true);

            // this probably isn't correct for all cases
            if (passengers.Count == 0)
                RemoveFromMap();
        }

        /// <summary>
        /// Invoked when <see cref="Player"/> is removed as a passenger from <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        protected virtual void OnPassengerRemove(Player player, VehicleSeatType seatType, byte seatPosition)
        {
            // deliberately empty
        }

        public IEnumerator<VehiclePassenger> GetEnumerator()
        {
            return passengers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
