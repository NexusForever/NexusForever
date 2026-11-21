using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingOwnership)]
    public class ServerHousingOwnership : IWritable
    {
        public Identity ResidenceIdentity { get; set; }
        public OwnershipType Ownership { get; set; } 

        public void Write(GamePacketWriter writer)
        {
            ResidenceIdentity.Write(writer);
            writer.Write(Ownership, 32u);
        }
    }
}
