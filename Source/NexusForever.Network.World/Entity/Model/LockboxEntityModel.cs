using NexusForever.Network.Message;

namespace NexusForever.Network.World.Entity.Model
{
    public class LockboxEntityModel : IEntityModel
    {
        public class UnknownLockboxStructure : IWritable
        {
            public uint Unknown0 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
            }
        }

        public uint CreatureId { get; set; }
        public List<UnknownLockboxStructure> Unknown1 { get; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CreatureId, 18);
            writer.Write((ushort)Unknown1.Count, 16u);
            Unknown1.ForEach(s => s.Write(writer));
        }
    }
}
