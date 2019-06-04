using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server0810)]
    public class Server0810 : IWritable
    {
        public class SpellTarget : IWritable
        {
            public uint ServerUniqueId { get; set; }
            public uint TargetId { get; set; }
            public uint InstanceCount { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(ServerUniqueId);
                writer.Write(TargetId);
                writer.Write(InstanceCount);
            }
        }

        public List<SpellTarget> spellTargets { get; set; } = new List<SpellTarget>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(spellTargets.Count, 32u);
            spellTargets.ForEach(i => i.Write(writer));
        }
    }
}
