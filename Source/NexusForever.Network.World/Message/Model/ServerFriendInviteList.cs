using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerFriendInviteList)]
    public class ServerFriendInviteList : IWritable
    {
        public class InviteData : IWritable
        {
            public ulong InviteId { get; set; }
            public Identity PlayerIdentity { get; set; }
            public uint Seen { get; set; }
            public float ExpiryInDays { get; set; }
            public string Note { get; set; }
            public string Name { get; set; }
            public Class Class { get; set; } 
            public Game.Static.Entity.Path Path { get; set; }
            public byte Level { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(InviteId);
                PlayerIdentity.Write(writer);
                writer.Write(Seen, 3u);
                writer.Write(ExpiryInDays);
                writer.WriteStringWide(Note);
                writer.WriteStringWide(Name);
                writer.Write(Class, 14u);
                writer.Write(Path, 3u);
                writer.Write(Level);
            }
        }

        public List<InviteData> Invites { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Invites.Count, 16u);
            Invites.ForEach(f => f.Write(writer));
        }
    }
}
