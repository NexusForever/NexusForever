using NexusForever.Game.Static.Quest;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestPeriodicReset)]
    public class ServerQuestPeriodicReset : IWritable
    {
        public ulong DailyRandomSeed { get; set; }
        public QuestRepeatPeriodFlags ResetFlags { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DailyRandomSeed);
            writer.Write(ResetFlags, 4u);
        }
    }
}
