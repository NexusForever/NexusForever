using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerExperienceGained)]
    public class ServerExperienceGained : IWritable
    {
        public uint TotalXpGained { get; set; }
        public uint RestXpAmountSpent { get; set; }
        public uint SignatureXpAmount { get; set; }
        public ExpReason Reason { get; set; }
        public uint UnitTarget { get; set; } // unused

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TotalXpGained);
            writer.Write(RestXpAmountSpent);
            writer.Write(SignatureXpAmount);
            writer.Write(Reason, 32u);
            writer.Write(UnitTarget);
        }
    }
}
