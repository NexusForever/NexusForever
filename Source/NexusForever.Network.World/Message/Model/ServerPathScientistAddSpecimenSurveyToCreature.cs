using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathScientistAddSpecimenSurveyToCreature)]
    public class ServerPathScientistAddSpecimenSurveyToCreature : IWritable
    {
        public uint UnitId { get; set; }
        public uint PathScientistSpecimenSurveyId { get; set; }
        public uint ObjectiveIndex { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            writer.Write(PathScientistSpecimenSurveyId, 14);
            writer.Write(ObjectiveIndex);
        }
    }   
}
