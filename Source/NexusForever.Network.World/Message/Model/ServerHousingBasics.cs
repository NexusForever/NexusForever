using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingBasics)]
    public class ServerHousingBasics : IWritable
    {
        [Flags]
        public enum ResidencePrivacyLevelFlags
        {
            Public        = 0x00,
            Private       = 0x01,
            NeighborsOnly = 0x02,
            RoommatesOnly = 0x04
        }

        public ulong ResidenceId { get; set; }
        public ulong NeighbourhoodId { get; set; }
        public ResidencePrivacyLevelFlags PrivacyLevel { get; set; }
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(ResidenceId);
            writer.Write(NeighbourhoodId);
            writer.Write(PrivacyLevel, 32u);
        }
    }
}
