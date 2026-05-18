using NexusForever.Game.Static.Pvp;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pvp
{
    // Send to all observers in the nearby area. Client handles the correctness of the UI messages.
    [Message(GameMessageOpcode.ServerDuelResult)]
    public class ServerDuelResult : IWritable
    {
        public uint WinnerUnitId { get; set; }
        public uint LoserUnitId { get; set; }
        public DuelFinishReason Reason { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(WinnerUnitId);
            writer.Write(LoserUnitId);
            writer.Write(Reason, 3u);
        }
    }
}
