using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildRoster)]
    public class ServerGuildRoster : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public List<GuildMember> GuildMembers { get; set; } = new();
        public bool Done { get; set; } = true; // Indicates if the roster is complete (true) or if more members are expected (false)
                                                // Possibly used for rate limiting the information flow or dealing with packets that grow too large, TBD

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(GuildMembers.Count);
            GuildMembers.ForEach(w => w.Write(writer));
            writer.Write(Done);
        }
    }
}
