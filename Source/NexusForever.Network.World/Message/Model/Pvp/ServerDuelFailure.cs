using NexusForever.Game.Static.Pvp;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    [Message(GameMessageOpcode.ServerDuelFailure)]
    public class ServerDuelFailure : IWritable
    {
        public DuelFailureReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Reason, 5u);
        }
    }
}
