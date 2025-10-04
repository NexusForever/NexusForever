using NexusForever.Game.Static.Guild;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildClassification)]
    public class ServerGuildClassification : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public GuildClassification Classification { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(Classification, 14u);
        }
    }
}
