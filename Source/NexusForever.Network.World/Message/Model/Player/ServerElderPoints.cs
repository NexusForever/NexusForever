using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerElderPoints)]
    public class ServerElderPoints : IWritable
    {
        public uint TotalPointsGained { get; set; } // includes rested and signature
        public uint CurrentTotal { get; set; }
        public uint CurrentDailyPoints { get; set; }
        public uint RestedPointsGained { get; set; }
        public uint SignaturePointsGained { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TotalPointsGained);
            writer.Write(CurrentTotal);
            writer.Write(CurrentDailyPoints);
            writer.Write(RestedPointsGained);
            writer.Write(SignaturePointsGained);
        }
    }
}
