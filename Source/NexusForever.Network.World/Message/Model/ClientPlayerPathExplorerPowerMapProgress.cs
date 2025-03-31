using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientPlayerPathExplorerPowerMapProgress)]
    public class ClientPlayerPathExplorerPowerMapProgress : IReadable
    {
        public uint PathMissionId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PathMissionId = reader.ReadUInt(14);
        }
    }
}
