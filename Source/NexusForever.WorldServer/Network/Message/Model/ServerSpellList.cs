using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAbilities)]
    public class ServerSpellList : IWritable
    {
        public class Spell : IWritable
        {   
            public uint Spell4BaseId { get; set; }
            public byte TierIndexAchieved { get; set; }
            public byte SpecIndex { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Spell4BaseId, 18u);
                writer.Write(TierIndexAchieved, 4u);
                writer.Write(SpecIndex, 3u);
            }
        }

        public List<Spell> Spells { get; set; } = new List<Spell>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write((uint)Spells.Count);
            Spells.ForEach(e => e.Write(writer));
        }
    }
}
