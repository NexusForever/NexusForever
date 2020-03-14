using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Quest.Static;

namespace NexusForever.WorldServer.Network.Message.Model
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
                public uint Progress { get; set; }
                public uint Timer { get; set; }
            }

            public ushort QuestId { get; set; }
            public QuestState State { get; set; }
            public uint RandomResultId { get; set; }
            public QuestFlags Flags { get; set; }
            public List<Objective> Objectives { get; set; } = new List<Objective>();
            public uint Timer { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(QuestId, 15u);
                writer.Write(State, 4u);
                writer.Write(RandomResultId);
                writer.Write(Flags, 32u);

                writer.Write(Objectives.Count);

                foreach (Objective objective in Objectives)
                    writer.Write(objective.Progress);

                writer.Write(Timer);

                foreach (Objective objective in Objectives)
                    writer.Write(objective.Timer);
            }
        }

        public List<QuestComplete> Completed { get; set; } = new List<QuestComplete>();
        public List<QuestInactive> Inactive { get; set; } = new List<QuestInactive>();
        public List<QuestActive> Active { get; set; } = new List<QuestActive>();

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
