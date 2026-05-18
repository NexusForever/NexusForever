using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingResult)]
    public class ServerHousingResult : IWritable
    {
        public ushort RealmId { get; set; }
        public ulong ResidenceId { get; set; }
        public string PlayerName { get; set; }
        public HousingResult Result { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RealmId, 14u);
            writer.Write(ResidenceId);
            writer.WriteStringWide(PlayerName);
            writer.Write(Result, 7u);
        }
    }
}
