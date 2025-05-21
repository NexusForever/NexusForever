using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerQuestShareResult)]
    public class ServerQuestShareResult : IWritable
    {
        public ushort QuestId { get; set; }
        public byte Viewpoint { get; set; }
        public uint Reason { get; set; } // TODO: Enum
        public uint UnitId { get; set; } // Depends on viewpoint what the client gets

        public void Write(GamePacketWriter writer)
        {
            writer.Write(QuestId, 15);
            writer.Write(Viewpoint, 3);
            writer.Write(Reason);
            writer.Write(UnitId);
        }
    }
}
