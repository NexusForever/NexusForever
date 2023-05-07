using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsUpdateStatus)]
    public class ServerContactsUpdateStatus : IWritable
    {
        public TargetPlayerIdentity PlayerIdentity { get; set; }
        public float LastOnlineInDays { get; set; }

        public void Write(GamePacketWriter writer)
        {
            PlayerIdentity.Write(writer);
            writer.Write(LastOnlineInDays);
        }
    }
}
