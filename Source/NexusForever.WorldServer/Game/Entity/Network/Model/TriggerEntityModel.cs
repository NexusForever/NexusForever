using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Game.Entity.Network.Model
{
    public class TriggerEntityModel : IEntityModel
    {
        public class UnknownTriggerStructure : IWritable
        {
            public byte Unknown0 { get; set; }
            public Position Position { get; set; }
            public Position Rotation { get; set; }
            public uint Unknown1 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0, 2);
                Position.Write(writer);
                Rotation.Write(writer);
                writer.Write(Unknown1);
            }
        }

        public string Name { get; set; }
        public List<UnknownTriggerStructure> Unknown0 { get; } = new List<UnknownTriggerStructure>();

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(Name);
            writer.Write((byte)Unknown0.Count, 8);
            Unknown0.ForEach(s => s.Write(writer));
        }
    }
}
