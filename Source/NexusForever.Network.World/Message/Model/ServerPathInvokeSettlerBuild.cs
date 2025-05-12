using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Fires whenever a settler interacts with a settler depot.
    [Message(GameMessageOpcode.ServerPathInvokeSettlerBuild)]
    public class ServerPathInvokeSettlerBuild : IWritable
    {
        public uint SettlerHubUnitId { get; set; }
        public uint[] PathSettlerImprovementGroupIds { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SettlerHubUnitId);
            writer.Write(PathSettlerImprovementGroupIds.Length);
            for (uint i = 0; i < PathSettlerImprovementGroupIds.Length; i++)
            {
                writer.Write(PathSettlerImprovementGroupIds[i]);
            }   
        }
    }
}
