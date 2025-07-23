using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Player
{
    [Message(GameMessageOpcode.ServerPrimeLevelsAchieved)]
    public class ServerPrimeLevelsAchieved : IWritable
    {
        public class PrimeLevelAchieved : IWritable
        {
            public uint WorldId { get; set; }
            public ushort LevelAchieved { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(WorldId);
                writer.Write(LevelAchieved);
            }
        }

        public List<PrimeLevelAchieved> PrimeLevelsAchieved { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PrimeLevelsAchieved.Count);
            PrimeLevelsAchieved.ForEach(level => level.Write(writer));
        }
    }
}
