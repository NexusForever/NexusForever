using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerP2PTradeResult)]
    public class ServerP2PTradeResult : IWritable
    {
        public enum P2PTradeResult
        {
            ErrorInitiating = 0,
            PlayerLogOut = 1,
            PlayerCanceled = 2,
            PlayerAcceptedInvite = 3,
            PlayerDeclinedInvite = 4,
            MissingPlayer = 5,
            InitiatorCommitted = 6,
            InitiatorUnCommitted = 7,
            TargetCommitted = 8,
            TargetUnCommitted = 9,
            NothingToTrade = 10,
            FinishedSuccess = 11,
            DbFailedToInit = 12,
            DbFailed = 13,
            ErrorAddingItem = 14,
            ErrorRemovingItem = 15,
            TargetBusy = 16,
            TargetNotAllowedToTrade = 17,
            InviteFailedNoTrading = 18
        }

        public P2PTradeResult Result { get; set; }
        public bool Cancelled { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result, 5);
            writer.Write(Cancelled);
        }
    }
}
