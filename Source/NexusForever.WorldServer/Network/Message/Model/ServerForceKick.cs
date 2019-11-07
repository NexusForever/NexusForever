using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
