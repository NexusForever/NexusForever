using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Entity.Model
{
    public class MountEntityModel : IEntityModel
    {
        public uint Creature2Id { get; set; }
        public ushort UnitVehicleId { get; set; }
        public uint OwnerUnitId { get; set; }
        public List<VehiclePassenger> Passengers { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Creature2Id, 18u);
            writer.Write(UnitVehicleId, 14u);
            writer.Write(OwnerUnitId);

            writer.Write((byte)Passengers.Count, 3u);
            Passengers.ForEach(p => p.Write(writer));
        }
    }
}
