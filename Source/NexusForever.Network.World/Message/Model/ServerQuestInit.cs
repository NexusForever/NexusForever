using NexusForever.Game.Static.Quest;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestInit)]
    public class ServerQuestInit : IWritable
    {
        public class QuestComplete : IWritable
        {
            public ushort QuestId { get; set; }
            public bool CompletedToday { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(QuestId, 15u);
                writer.Write(CompletedToday);
            }
        }

        public class QuestInactive : IWritable
        {
            public ushort QuestId { get; set; }
            public QuestState State { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(QuestId, 15u);
                writer.Write(State, 4u);
            }
        }

        public class QuestActive : IWritable
        {
            public class Objective
            {
                public uint Progress { get; set; } // Sometimes flags, sometimes numbers
                public uint TimeElapsed { get; set; }
            }

            public ushort QuestId { get; set; }
            public QuestState State { get; set; }
            public uint QuestObjectiveId { get; set; } // See questObjective tbl
            public QuestStateFlags Flags { get; set; }
            public List<Objective> Objectives { get; set; } = new();
            public uint QuestTimeElapsed { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(QuestId, 15u);
                writer.Write(State, 4u);
                writer.Write(QuestObjectiveId);
                writer.Write(Flags, 32u);

                writer.Write(Objectives.Count);

                foreach (Objective objective in Objectives)
                    writer.Write(objective.Progress);

                writer.Write(QuestTimeElapsed);

                foreach (Objective objective in Objectives)
                    writer.Write(objective.TimeElapsed);
            }
        }

        public List<QuestComplete> Completed { get; set; } = new();
        public List<QuestInactive> Inactive { get; set; } = new();
        public List<QuestActive> Active { get; set; } = new();

        public ulong DailyRandomSeed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Completed.Count);
            Completed.ForEach(q => q.Write(writer));

            writer.Write(Inactive.Count);
            Inactive.ForEach(q => q.Write(writer));

            writer.Write(Active.Count);
            Active.ForEach(q => q.Write(writer));

            writer.Write(DailyRandomSeed);
        }
    }
}
