using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellInterrupted2)]
    public class ServerSpellInterrupted2 : IWritable
    {
        public uint CastingId { get; set; }
        public CastResult Spell4CastResultId { get; set; }
        public uint CasterUnitId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Spell4CastResultId, 9u);
            writer.Write(CasterUnitId);
        }
    }
}

