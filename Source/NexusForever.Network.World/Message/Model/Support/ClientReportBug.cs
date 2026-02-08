using NexusForever.Game.Static.Support;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Support
{
    [Message(GameMessageOpcode.ClientReportBug)]
    public class ClientReportBug : IReadable
    {
        public BugCategory BugCategoryId { get; private set; }
        public uint SelectedUnitId { get; private set; }
        public uint Quest2Id { get; private set; }
        public string Description { get; private set; }

        public void Read(GamePacketReader reader)
        {
            BugCategoryId = reader.ReadEnum<BugCategory>(16u);
            SelectedUnitId = reader.ReadUInt();
            Quest2Id = reader.ReadUInt();
            Description = reader.ReadWideString();
        }
    }
}
