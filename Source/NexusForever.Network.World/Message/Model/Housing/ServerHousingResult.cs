using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingResult)]
    public class ServerHousingResult : IWritable
    {
        public Identity ResidenceIdentity { get; set; } // not used by client
        public string PlayerName { get; set; }
        public HousingResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            ResidenceIdentity.Write(writer);
            writer.WriteStringWide(PlayerName);
            writer.Write(Result, 7u);
        }
    }
}
