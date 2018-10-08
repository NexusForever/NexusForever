using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class MountEntityModel : IEntityModel
    {
        // probably flairs
        public class UnknownMountStructure : IWritable
        {
            public byte Unknown0 { get; set; }
            public byte Unknown1 { get; set; }
            public uint Unknown2 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 2);
                writer.Write(Unknown1, 3);
                writer.Write(Unknown2);
            }
        }

        public uint CreatureId { get; set; }
        public ushort Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public List<UnknownMountStructure> Unknown4 { get; } = new List<UnknownMountStructure>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write(Unknown1, 14);
            writer.Write(Unknown2);

            writer.Write((byte)Unknown4.Count, 3);
            Unknown4.ForEach(s => s.Write(writer));
        }
    }
}
