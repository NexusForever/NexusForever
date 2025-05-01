using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerTradeskillRelearnCooldownUpdate)]
    public class ServerTradeskillRelearnCooldownUpdate : IWritable
    {
        public uint RelearnCooldown { get; set; } // Sent as an offset from the time now, to the finish time, in milliseconds.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RelearnCooldown);
        }
    }
}
