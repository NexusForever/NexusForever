using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Guild
{
    [Message(GameMessageOpcode.ServerGuildPerkActivated)]
    public class ServerGuildPerkActivated : IWritable
    {
        public Identity GuildIdentity { get; set; }
        public ushort GuildPerkId { get; set; }
        public float DaysAgoActivated { get; set; }

        public void Write(GamePacketWriter writer)
        {
            GuildIdentity.Write(writer);
            writer.Write(GuildPerkId, 14u);
            writer.Write(DaysAgoActivated);
        }
    }
}
