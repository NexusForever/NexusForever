using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingRandomCommunityList)]
    public class ServerHousingRandomCommunityList : IWritable
    {
        public class Community : IWritable
        {
            public ushort RealmId { get; set; }
            public ulong NeighborhoodId { get; set; }
            public string Name { get; set; }
            public string Owner { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(RealmId, 14u);
                writer.Write(NeighborhoodId);
                writer.WriteStringWide(Name);
                writer.WriteStringWide(Owner);
            }
        }

        public List<Community> Communities { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Communities.Count);
            Communities.ForEach(c => c.Write(writer));
        }
    }
}
