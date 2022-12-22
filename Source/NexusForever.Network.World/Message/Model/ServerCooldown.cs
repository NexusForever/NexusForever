using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
