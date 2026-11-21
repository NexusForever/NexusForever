using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.PlayerPath
{
    [Message(GameMessageOpcode.ClientPathExplorerPowerMapProgress)]
    public class ClientPathExplorerPowerMapProgress : IReadable
    {
        public uint PathMissionId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            PathMissionId = reader.ReadUInt(14);
        }
    }
}
