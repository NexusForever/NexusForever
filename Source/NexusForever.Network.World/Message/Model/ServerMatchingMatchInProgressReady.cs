using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerMatchingMatchInProgressReady)]

    // If player is in a match, fires MatchingGamePendingUpdate event for ally count update
    // If player is not in a match, fires MatchingGameReady event for a match that is in not started
    public class ServerMatchingMatchInProgressReady : IWritable
    {
        public Game.Static.Matching.MatchType MatchType { get; set; }
        public uint UnacceptedAllies { get; set; }
        public uint AcceptedAllies { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(MatchType, 5u);
            writer.Write(UnacceptedAllies);
            writer.Write(AcceptedAllies);
        }
    }
}
