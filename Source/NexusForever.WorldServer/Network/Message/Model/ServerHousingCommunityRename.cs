using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingCommunityRename)]
    public class ServerHousingCommunityRename : IWritable
    {
        public TargetGuild TargetGuild { get; set; }
        public HousingResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            TargetGuild.Write(writer);
            writer.Write(Result, 7u);
        }
    }
}
