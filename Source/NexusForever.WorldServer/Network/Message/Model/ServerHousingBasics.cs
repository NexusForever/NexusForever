using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using System;

namespace NexusForever.WorldServer.Network.Message.Model
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
