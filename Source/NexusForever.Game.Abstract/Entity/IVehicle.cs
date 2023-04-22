﻿using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IVehicle : IWorldEntity, IEnumerable<IVehiclePassenger>
    {
        Creature2Entry CreatureEntry { get; }
        UnitVehicleEntry VehicleEntry { get; }
        Spell4Entry SpellEntry { get; }

        /// <summary>
        /// Return <see cref="IVehiclePassenger"/> with supplied guid.
        /// </summary>
        IVehiclePassenger GetPassenger(uint guid);

        /// <summary>
        /// Return <see cref="IVehiclePassenger"/> with supplied <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        IVehiclePassenger GetPassenger(VehicleSeatType seatType, byte seatPosition);

        /// <summary>
        /// Enqueue <see cref="IPlayer"/> to be added as a passenger with supplied <see cref="VehicleSeatType"/> and seat position.
        /// </summary>
        void EnqueuePassengerAdd(IPlayer player, VehicleSeatType seatType, byte seatPosition);

        /// <summary>
        /// Remove <see cref="IPlayer"/> as a passenger.
        /// </summary>
        void PassengerRemove(IPlayer player);
    }
}