using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingPrivacy)]
    public class ServerHousingPrivacy : IWritable
    {
        public ulong ResidenceId { get; set; }
        public ulong NeighbourhoodId { get; set; }
        public ResidencePrivacyLevel PrivacyLevel { get; set; }
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(ResidenceId);
            writer.Write(NeighbourhoodId);
            writer.Write(PrivacyLevel, 32u);
        }
    }
}
