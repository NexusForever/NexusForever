using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellCastResult)]
    public class ServerSpellCastResult : IWritable
    {
        public uint Unknown0 { get; set; }
        public uint Spell4Id { get; set; }
        public CastResult CastResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(Spell4Id, 18u);
            writer.Write(CastResult, 9u);
        }
    }
}
