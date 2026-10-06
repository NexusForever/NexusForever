using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    public class Neighbour : IWritable
    {
        public ulong NeighbourId { get; set; }
        public ulong Unused { get; set; } // is also a NeighbourId but not used by client, roommate maybe?
        public Identity PlayerIdentity { get; set; }
        public uint Permissions { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(NeighbourId);
            writer.Write(Unused);
            PlayerIdentity.Write(writer);
            writer.Write(Permissions);
        }
    }
}
