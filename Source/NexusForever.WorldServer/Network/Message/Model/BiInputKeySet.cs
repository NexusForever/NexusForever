using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.BiInputKeySet)]
    public class BiInputKeySet : IReadable, IWritable
    {
        public List<Binding> Bindings { get; set; } = new List<Binding>();
        public ulong CharacterId { get; set; }

        public void Read(GamePacketReader reader)
        {
            uint count = reader.ReadUInt();
            for (uint i = 0u; i < count; i++)
            {
                var binding = new Binding();
                binding.Read(reader);
                Bindings.Add(binding);
            }

            CharacterId = reader.ReadULong();
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Bindings.Count);
            Bindings.ForEach(b => b.Write(writer));
            writer.Write(CharacterId);
        }
    }
}
