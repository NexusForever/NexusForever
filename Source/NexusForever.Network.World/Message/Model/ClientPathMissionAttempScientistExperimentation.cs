using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathMission_AttempScientistExperimentation)]
    public class ClientPathMission_AttempScientistExperimentation : IReadable
    {
        public List<uint> Choices { get; } = new(); // Are all PathScientistExperimentationPatternDataId

        public void Read(GamePacketReader reader)
        {
            for (uint i = 0u; i < 4 ; i++)
                Choices.Add(reader.ReadUInt());
        }
    }
}
