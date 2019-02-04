using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class TaxiEntityModel : IEntityModel
    {
        // probably list of passengers - list of 0x086F
        public class UnknownMountStructure : IWritable
        {
            public byte Unknown0 { get; set; }
            public byte Unknown1 { get; set; }
            public uint UnitId { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 2);
                writer.Write(Unknown1, 3);
                writer.Write(UnitId);
            }
        }

        public uint CreatureId { get; set; }
        public ushort UnitVehicleId { get; set; }
        public uint OwnerId { get; set; }
        public List<UnknownMountStructure> Unknown4 { get; } = new List<UnknownMountStructure>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write(UnitVehicleId, 14);
            writer.Write(OwnerId);

            writer.Write((byte)Unknown4.Count, 3);
            Unknown4.ForEach(s => s.Write(writer));
        }
    }
}
