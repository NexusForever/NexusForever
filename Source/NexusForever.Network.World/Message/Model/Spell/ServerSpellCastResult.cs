using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellCastResult)]
    public class ServerSpellCastResult : IWritable
    {
        public uint ClientSpellCastUniqueId { get; set; }
        public uint Spell4Id { get; set; }
        public CastResult CastResult { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ClientSpellCastUniqueId);
            writer.Write(Spell4Id, 18u);
            writer.Write(CastResult, 9u);
        }
    }
}
