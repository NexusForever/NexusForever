using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerForceKick)]
    public class ServerForceKick : IWritable
    {
        public ForceKickReason Reason { get; set; } = ForceKickReason.WorldDisconnect;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Reason, 5u);
        }
    }
}
