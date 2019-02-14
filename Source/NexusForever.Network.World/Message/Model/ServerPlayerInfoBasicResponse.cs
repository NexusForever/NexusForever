using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPlayerInfoBasicResponse)]
    public class ServerPlayerInfoBasicResponse : IWritable
    {
        public byte ResultCode { get; set; }
        public TargetPlayerIdentity Identity { get; set; }
        public string Name { get; set; }
        public Faction Faction { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(ResultCode, 3u);
            Identity.Write(writer);
            writer.WriteStringFixed(Name);
            writer.Write(Faction, 14u);
        }
    }
}
