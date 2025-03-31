using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPathExplorerProgressReport)]
    public class ClientPathExplorerProgressReport : IReadable
    {
        public uint PathMissionId { get; private set; }
        public uint ExplorerNodeIndex { get; private set; } // Reports when player unit has reached radius threshold around WorldLocation.

        public void Read(GamePacketReader reader)
        {
            PathMissionId = reader.ReadUInt(15);
            ExplorerNodeIndex = reader.ReadUInt();
        }
    }
}
