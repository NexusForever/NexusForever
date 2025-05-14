using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPathSettlerBuildResult)]
    public class ServerPathSettlerBuildResult : IWritable
    {
        public uint eResult { get; set; } 
        public uint Unknown { get; set; } // If this is not zero, client gets localizedTextId from PathSettlerImproveGroup
                                          // tbl entry to pass name of improvement that was build to SetterBuildResult event.
        public uint PathSettlerImprovementGroupId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(eResult);
            writer.Write(Unknown, 15);
            writer.Write(PathSettlerImprovementGroupId, 14);
        }
    }
}
