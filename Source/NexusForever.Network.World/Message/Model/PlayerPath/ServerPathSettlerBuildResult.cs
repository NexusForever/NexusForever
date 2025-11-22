using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ServerPathSettlerBuildResult)]
    public class ServerPathSettlerBuildResult : IWritable
    {
        public uint Result { get; set; }  // Is an enum but was never used in client. Passed to SetterBuildResult lua event.

        public uint Unknown { get; set; } // If this is not zero, client gets localizedTextId from PathSettlerImproveGroup
                                          // tbl entry to pass name of improvement that was build to SetterBuildResult event.
        public uint PathSettlerImprovementGroupId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Result);
            writer.Write(Unknown, 15);
            writer.Write(PathSettlerImprovementGroupId, 14);
        }
    }
}
