using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingCommunityRename)]
    public class ServerHousingCommunityRename : IWritable
    {
        public Identity TargetGuild { get; set; } // Not used in the client
        public HousingResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            TargetGuild.Write(writer);
            writer.Write(Result, 7u);
        }
    }
}
