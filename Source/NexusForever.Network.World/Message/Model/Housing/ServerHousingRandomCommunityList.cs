using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingRandomCommunityList)]
    public class ServerHousingRandomCommunityList : IWritable
    {
        public class Community : IWritable
        {
            public Identity GuildIdentity { get; set; }
            public string Name { get; set; }
            public string CommunityLeader { get; set; }

            public void Write(GamePacketWriter writer)
            {
                GuildIdentity.Write(writer);
                writer.WriteStringWide(Name);
                writer.WriteStringWide(CommunityLeader);
            }
        }

        public List<Community> Communities { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Communities.Count);
            Communities.ForEach(c => c.Write(writer));
        }
    }
}
