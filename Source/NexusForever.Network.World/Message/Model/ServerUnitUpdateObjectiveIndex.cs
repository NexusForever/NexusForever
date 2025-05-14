using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    /// <summary>
    /// ObjectiveIndex is used for PathMissions, Quests, Challenges, Datacubes, PublicEvents, Achievements.
    /// </summary>
    [Message(GameMessageOpcode.ServerUnitUpdateObjectiveIndex)]
    public class ServerUnitUpdateObjectiveIndex : IWritable
    {
        public uint UnitId { get; set; }
        public uint ObjectiveIndex { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(ObjectiveIndex);
        }
    }
}
