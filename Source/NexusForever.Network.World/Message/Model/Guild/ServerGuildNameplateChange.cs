using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    // Activates the guild nameplate for the player. This can be the nameplate
    // of any guild the player is a member of.
    [Message(GameMessageOpcode.ServerGuildNameplateChange)]
    public class ServerGuildNameplateChange: IWritable
    {
        public Identity GuildIdentity { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
        }
    }
}
