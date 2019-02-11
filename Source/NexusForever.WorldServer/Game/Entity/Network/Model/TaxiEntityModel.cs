using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class TaxiEntityModel : IEntityModel
    {
        public class Passenger : IWritable
        {
            public byte SeatType { get; set; }
            public byte SeatPosition { get; set; }
            public uint UnitId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(SeatType, 2);
                writer.Write(SeatPosition, 3);
                writer.Write(UnitId);
            }
        }

        public uint CreatureId { get; set; }
        public ushort UnitVehicleId { get; set; }
        public uint OwnerId { get; set; }
        public List<Passenger> Passengers { get; } = new List<Passenger>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write(UnitVehicleId, 14);
            writer.Write(OwnerId);

            writer.Write((byte)Passengers.Count, 3);
            Passengers.ForEach(s => s.Write(writer));
        }
    }
}
