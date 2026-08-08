using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    [Message(GameMessageOpcode.ServerHousingBuildState)]
    public class ServerHousingBuildState : IWritable
    {
        public Identity ResidenceIdentity { get; set; }
        public uint HousingPlotIndex { get; set; }
        public uint BuildStage { get; set; } // changes the model sequence for the prop, see tbl housingBuild.modelSequenceId
        public BuildState State { get; set; }

        public void Write(GamePacketWriter writer)
        {
            ResidenceIdentity.Write(writer);
            writer.Write(HousingPlotIndex);
            writer.Write(BuildStage);
            writer.Write(State, 3u);
        }
    }
}
