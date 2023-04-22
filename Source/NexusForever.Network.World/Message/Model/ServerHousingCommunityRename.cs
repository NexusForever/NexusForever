using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
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
