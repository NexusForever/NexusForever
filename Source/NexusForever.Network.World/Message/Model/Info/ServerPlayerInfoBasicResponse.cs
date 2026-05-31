using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Info
{
    [Message(GameMessageOpcode.ServerPlayerInfoBasicResponse)]
    public class ServerPlayerInfoBasicResponse : IWritable
    {
        public PlayerInfoBase BaseData { get; set; }

        public void Write(GamePacketWriter writer)
        {
            BaseData.Write(writer);
        }
    }
}
