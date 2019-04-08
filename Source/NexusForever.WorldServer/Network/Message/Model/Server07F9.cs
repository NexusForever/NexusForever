using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07F9)]
    public class Server07F9 : IWritable
    {
        public uint ServerUniqueId { get; set; }
        public CastResult CastResult { get; set; }
        public uint CasterId { get; set; }
        public bool CancelCast { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ServerUniqueId);
            writer.Write(CastResult, 9u);
            writer.Write(CasterId);
            writer.Write(CancelCast);
        }
    }
}
