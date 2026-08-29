using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathScientistExperimentationInvoke)]
    public class ServerPathScientistExperimentationInvoke : IWritable
    {
        public uint UnitId { get; set; }
        public uint[] CurrentPattern { get; set; } = new uint[4]; // PathScientistExperimentationPatternDataId

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
