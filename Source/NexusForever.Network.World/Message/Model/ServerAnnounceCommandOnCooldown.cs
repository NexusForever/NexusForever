using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerAnnounceCommandOnCooldown)]
    public class ServerAnnounceCommandOnCooldown : IWritable
    {
        public uint CooldownTime { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CooldownTime);
        }
    }
}
