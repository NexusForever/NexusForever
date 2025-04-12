using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathMissionAttemptScientistExperimentation)]
    public class ClientPathMissionAttemptScientistExperimentation : IReadable
    {
        public List<uint> Choices { get; private set; } = new(); // Are all PathScientistExperimentationPatternDataId

        public void Read(GamePacketReader reader)
        {
            for (uint i = 0u; i < 4 ; i++)
                Choices.Add(reader.ReadUInt());
        }
    }
}
