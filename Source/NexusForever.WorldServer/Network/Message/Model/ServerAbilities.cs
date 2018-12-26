using System;
using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAbilities, MessageDirection.Server)]
    public class ServerAbilities : IWritable
    {
        public class Ability : IWritable
        {   
            public uint Spell4BaseId { get; set; }
            public byte Tier { get; set; }
            public byte SomeCounter { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Spell4BaseId, 18u);
                writer.Write(Tier, 4u);
                writer.Write(SomeCounter, 3u);
            }
        }

        public List<Ability> ability { get; set; } = new List<Ability>();
        public void Write(GamePacketWriter writer)
        {
            writer.Write((uint)ability.Count);
            ability.ForEach(e => e.Write(writer));
        }
    }
}
