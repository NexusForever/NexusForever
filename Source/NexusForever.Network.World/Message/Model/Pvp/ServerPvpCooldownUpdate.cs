using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // Time for PVP flag to expire in milliseconds
    [Message(GameMessageOpcode.ServerPvpCooldownUpdate)]
    public class ServerPvpCooldownUpdate : IWritable
    {
        public uint CooldownRemaining { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CooldownRemaining);
        }
    }
}
