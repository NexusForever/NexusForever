using System.Collections.Generic;
using NexusForever.Shared.Network;
using NetworkVehiclePassenger = NexusForever.WorldServer.Network.Message.Model.Shared.VehiclePassenger;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class VehicleEntityModel : IEntityModel
    {
        public uint CreatureId { get; set; }
        public ushort UnitVehicleId { get; set; }
        public List<NetworkVehiclePassenger> Passengers { get; set; } = new List<NetworkVehiclePassenger>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18u);
            writer.Write(UnitVehicleId, 14u);

            writer.Write((byte)Passengers.Count, 3u);
            Passengers.ForEach(p => p.Write(writer));
        }
    }
}
