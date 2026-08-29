using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Spell
{
    [Message(GameMessageOpcode.ServerSpellInterrupted1)]
    public class ServerSpellInterrupted1 : IWritable
    {
        public uint ServerUniqueId { get; set; }
        public CastResult CastResult { get; set; }
        public uint CasterUnitId { get; set; }
        public bool CancelCast { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(CastResult, 9u);
            writer.Write(CasterUnitId);
            writer.Write(CancelCast);
        }
    }
}
