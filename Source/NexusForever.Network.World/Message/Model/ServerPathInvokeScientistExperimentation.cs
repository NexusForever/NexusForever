using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathInvokeScientistExperimentation)]
    public class ServerPathInvokeScientistExperimentation : IWritable
    {
        public uint UnitId { get; set; }
        public uint[] CurrentPattern { get; set; } // PathScientistExperimentationPatternDataId

        public void Write(GamePacketWriter writer)
        {
            writer.Write(UnitId);
            for(int i = 0; i < 4; i++)
            {
                writer.Write(CurrentPattern[i]);
            }
        }
    }
}
