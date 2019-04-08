using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCooldown)]
    public class ServerCooldown : IWritable
    {
        public Cooldown Cooldown { get; set; }

        public void Write(GamePacketWriter writer)
        {
            Cooldown.Write(writer);
        }
    }
}
