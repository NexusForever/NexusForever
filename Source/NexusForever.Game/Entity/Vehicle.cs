using System.Collections;
using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Entity;
using NexusForever.Network.World.Entity.Model;
using NexusForever.Network.World.Message.Model;
using NetworkVehiclePassenger = NexusForever.Network.World.Message.Model.Shared.VehiclePassenger;

namespace NexusForever.Game.Entity
{
    public class Vehicle : WorldEntity, IVehicle
    {
        public UnitVehicleEntry VehicleEntry { get; }
        public Spell4Entry SpellEntry { get; }

        protected readonly List<IVehiclePassenger> passengers = new();
        private readonly Queue<IVehiclePassenger> pendingAdd = new();

        public Vehicle()
            : base(EntityType.Vehicle)
        {
            // TODO
        }

        protected Vehicle(EntityType type, uint creatureId, uint vehicleId, uint spell4Id)
            : base(type)
        {
            Initialise(creatureId, 0, 0);

            VehicleEntry  = GameTableManager.Instance.UnitVehicle.GetEntry(vehicleId != 0u ? vehicleId : CreatureEntry.UnitVehicleId);
            SpellEntry    = GameTableManager.Instance.Spell4.GetEntry(spell4Id);

            // temp
            SetBaseProperty(Property.BaseHealth, 800.0f);

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
            foreach (IVehiclePassenger passenger in passengers)
            {
                IPlayer entity = GetVisible<IPlayer>(passenger.Guid);
                PassengerRemove(entity, passenger);
            }

            base.OnRemoveFromMap();
        }

        public override void OnRelocate(Vector3 vector)
        {
            foreach (IVehiclePassenger passenger in passengers)
            {
                IPlayer entity = GetVisible<IPlayer>(passenger.Guid);
                Map.EnqueueRelocate(entity, vector);
            }

            base.OnRelocate(vector);
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            // passengers are delay added to make sure the vehicle exists at the client
            while (pendingAdd.TryDequeue(out IVehiclePassenger passenger))
                PassengerAdd(passenger);
        }

        /// <summary>
        /// Return <see cref="IVehiclePassenger"/> with supplied guid.
        /// </summary>
        public IVehiclePassenger GetPassenger(uint guid)
        {
            return passengers.SingleOrDefault(p => p.Guid == guid);
        }

        /// <summary>
        /// Return <see cref="IVehiclePassenger"/> with supplied <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        public IVehiclePassenger GetPassenger(VehicleSeatType seatType, byte seatPosition)
        {
            return passengers.SingleOrDefault(p => p.SeatType == seatType && p.SeatPosition == seatPosition);
        }

        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added as a passenger with supplied <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        public void EnqueuePassengerAdd(IPlayer player, VehicleSeatType seatType, byte seatPosition)
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

        private void PassengerAdd(IVehiclePassenger passenger)
        {
            // possible for a player to no longer be visible due to delayed add
            IPlayer player = GetVisible<IPlayer>(passenger.Guid);
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
        /// Invoked when <see cref="IPlayer"/> is added as a passenger to <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        protected virtual void OnPassengerAdd(IPlayer player, VehicleSeatType seatType, byte seatPosition)
        {
            // deliberately empty
        }

        /// <summary>
        /// Remove <see cref="IPlayer"/> as a passenger.
        /// </summary>
        public void PassengerRemove(IPlayer player)
        {
            IVehiclePassenger passenger = GetPassenger(player.Guid);
            if (passenger == null)
                throw new ArgumentException();

            PassengerRemove(player, passenger);
        }

        private void PassengerRemove(IPlayer player, IVehiclePassenger passenger)
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
        /// Invoked when <see cref="IPlayer"/> is removed as a passenger from <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        protected virtual void OnPassengerRemove(IPlayer player, VehicleSeatType seatType, byte seatPosition)
        {
            // deliberately empty
        }

        public IEnumerator<IVehiclePassenger> GetEnumerator()
        {
            return passengers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
