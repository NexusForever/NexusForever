using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Crafting
{
    [Message(GameMessageOpcode.ServerTradeskillRelearnCooldown)]
    public class ServerTradeskillRelearnCooldown : IWritable
    {
        public uint RelearnCooldown { get; set; } // Sent as an offset from the time now, to the finish time, in milliseconds.

        public void Write(GamePacketWriter writer)
        {
            writer.Write(RelearnCooldown);
        }
    }
}
